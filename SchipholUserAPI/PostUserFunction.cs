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
                        cmd.CommandText = "insert into tblUser values (@UserId, @FName, @Name, @Email, @Password)";
                        cmd.Parameters.AddWithValue("@UserId", reg.UserId);
                        cmd.Parameters.AddWithValue("@FName", reg.Fname);
                        cmd.Parameters.AddWithValue("@Name", reg.Name);
                        cmd.Parameters.AddWithValue("@Email", reg.Email);
                        cmd.Parameters.AddWithValue("@Password", reg.PasswordHash);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return new OkObjectResult("");
            }
            catch (Exception ex)
            {
                log.LogError(ex + "     ---->AddRegistration");
                return new StatusCodeResult(500);
            }
        }
    }
}
