using System;
using Newtonsoft.Json;

namespace FlightMobileApp.Models
{
    public class Response
    {
        public Response()
        {
        }

        public Response(string response)
        {
            this.Message = response;
        }

        [JsonProperty("response")]
        public string Message { get; set; }
    }
}
