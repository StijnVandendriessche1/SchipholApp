using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SchipholUserAPI.Models
{
    public class Flight
    {
        [JsonProperty(PropertyName = "FlightID")]
        public string vluchtId { get; set; }

        [JsonProperty(PropertyName = "UserID")]
        public string gebruikerId { get; set; }

        [JsonProperty(PropertyName = "SavedID")]
        public int savedId { get; set; }
    }
}
