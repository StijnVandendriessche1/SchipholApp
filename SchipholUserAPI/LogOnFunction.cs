using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SchipholUserAPI.Models;
using System.Data.SqlClient;

namespace SchipholUserAPI
{
    public static class LogOnFunction
    {
        [FunctionName("LogOnFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            try
            {
                string constr = Environment.GetEnvironmentVariable("CONNECTIONSTRING");
                string json = await new StreamReader(req.Body).ReadToEndAsync();
                User reg = JsonConvert.DeserializeObject<User>(json);
                User u = new User();
                bool succes = false;

                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = constr;
                    await con.OpenAsync();
                    using(SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "select * from tblUser where Email = @email";
                        cmd.Parameters.AddWithValue("@email", reg.Email);
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        if(reader.HasRows)
                        {
                            while(reader.Read())
                            { 
                                u.UserId = reader["UserId"].ToString();
                                u.Fname = reader["FName"].ToString();
                                u.Name = reader["Name"].ToString();
                                u.Email = reg.Email;
                                u.Password = reader["Password"].ToString();
                                if(reg.PasswordHash == u.Password)
                                {
                                    succes = true;
                                }
                                else
                                {
                                    succes = false;
                                }
                            }
                        }
                        else
                        {
                            succes = false;
                        }
                    }
                }
                if(succes)
                {
                    return new OkObjectResult(u.UserId);
                }
                else
                {
                    return new OkObjectResult("FAIL");
                }
            }
            catch(Exception ex)
            {
                log.LogError(ex + "     ---->AddRegistration");
                return new StatusCodeResult(500);
            }
        }
    }
}
