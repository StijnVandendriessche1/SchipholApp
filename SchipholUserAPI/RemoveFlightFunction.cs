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
    public static class RemoveFlightFunction
    {
        [FunctionName("RemoveFlightFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "delete/{saveId}")] HttpRequest req, string saveId,
            ILogger log)
        {
            try
            {
                int savedId = Convert.ToInt32(saveId);
                string connectionstring = Environment.GetEnvironmentVariable("CONNECTIONSTRING");
                using (SqlConnection con = new SqlConnection())
                {
                    con.ConnectionString = connectionstring;
                    await con.OpenAsync();
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.CommandText = "DELETE FROM tblSaved WHERE SavedID = @id;";
                        cmd.Parameters.AddWithValue("@id", savedId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return new OkResult();
            }
            catch(Exception ex)
            {
                return new StatusCodeResult(500);
            }
        }
    }
}
