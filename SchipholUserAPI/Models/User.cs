using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace SchipholUserAPI.Models
{
    public class User
    {
        [JsonProperty(PropertyName = "UserId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "Fname")]
        public string Fname { get; set; }

        [JsonProperty(PropertyName = "Name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "Email")]
        public string Email { get; set; }

        [JsonProperty(PropertyName = "Password")]
        public string Password { get; set; }
        
        public string PasswordHash
        {
            get 
            { 
                return GetStringSha256Hash(Password); 
            }
        }

        internal static string GetStringSha256Hash(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

    }
}
