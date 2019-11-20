using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using SchipholUserAPI.Models;

namespace SchipholUserAPI
{
    public static class PostUserFunction
    {
        [FunctionName("PostUserFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "newUser")] HttpRequest req,
            ILogger log)
        {
            try
            {
                bool isUnique;
                string constr = Environment.GetEnvironmentVariable("CONNECTIONSTRING");
                string json = await new StreamReader(req.Body).ReadToEndAsync();
                User reg = JsonConvert.DeserializeObject<User>(json);
                reg.UserId = Guid.NewGuid().ToString();
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = constr;
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;

                        cmd.CommandText = "select * from tblUser where Email = @email";
                        cmd.Parameters.AddWithValue("@email", reg.Email);
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        if (reader.HasRows)
                        {
                            isUnique = false;
                        }
                        else
                        {
                            using(SqlConnection co = new SqlConnection())
                            {
                                co.ConnectionString = constr;
                                await co.OpenAsync();
                                using (SqlCommand c = new SqlCommand())
                                {
                                    c.Connection = co;
                                    c.CommandText = "insert into tblUser values (@UserId, @FName, @Name, @Email, @Password)";
                                    c.Parameters.AddWithValue("@UserId", reg.UserId);
                                    c.Parameters.AddWithValue("@FName", reg.Fname);
                                    c.Parameters.AddWithValue("@Name", reg.Name);
                                    c.Parameters.AddWithValue("@Email", reg.Email);
                                    c.Parameters.AddWithValue("@Password", reg.PasswordHash);
                                    await c.ExecuteNonQueryAsync();
                                    isUnique = true;
                                }
                            }
                        }
                    }
                }
                return new OkObjectResult(isUnique);
            }
            catch (Exception ex)
            {
                log.LogError(ex + "     ---->AddRegistration");
                return new StatusCodeResult(500);
            }
        }
    }
}
