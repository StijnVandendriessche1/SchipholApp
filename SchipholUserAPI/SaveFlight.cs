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
    public static class SaveFlight
    {
        [FunctionName("SaveFlight")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "SaveFlight")] HttpRequest req,
            ILogger log)
        {
            try
            {
                string constr = Environment.GetEnvironmentVariable("CONNECTIONSTRING");
                string json = await new StreamReader(req.Body).ReadToEndAsync();
                Flight flight = JsonConvert.DeserializeObject<Flight>(json);
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = constr;
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "insert into tblSaved values (@FlightID, @UserID)";
                        cmd.Parameters.AddWithValue("@FlightID", flight.vluchtId);
                        cmd.Parameters.AddWithValue("@UserID", flight.gebruikerId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return new StatusCodeResult(200);
            }
            catch(Exception ex)
            {
                log.LogError(ex + "     ---->AddRegistration");
                return new StatusCodeResult(500);
            }
        }
    }
}
