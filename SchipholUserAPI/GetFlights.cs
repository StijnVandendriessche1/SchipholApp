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
using System.Collections.Generic;

namespace SchipholUserAPI
{
    public static class GetFlights
    {
        [FunctionName("GetFlights")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "getFlights/{userid}")] HttpRequest req, string userid,
            ILogger log)
        {
            List<Flight> flights = new List<Flight>();
            try
            {
                string connectionstring = Environment.GetEnvironmentVariable("CONNECTIONSTRING");
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = connectionstring;
                    await con.OpenAsync();
                    using(SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "select * from tblSaved where UserID = @id";
                        cmd.Parameters.AddWithValue("@id", userid);
                        SqlDataReader reader = await cmd.ExecuteReaderAsync();
                        
                            while(reader.Read())
                            {
                                Flight f = new Flight();
                                f.vluchtId = reader["FlightID"].ToString();
                                f.gebruikerId = reader["UserID"].ToString();
                            f.savedId = Convert.ToInt32(reader["SavedID"]);
                            flights.Add(f);
                            }
                        return new OkObjectResult(flights);
                    }
                }
            }
            catch(Exception ex)
            {
                log.LogError(ex + "     ---->GetFlights");
                return new StatusCodeResult(500);
            }
        }
    }
}
