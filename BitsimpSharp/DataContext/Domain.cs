using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitsimpBot.DataContext
{
    public class Domain
    {
        [JsonProperty("domain")]
        public string domain { get; set; }
        [JsonProperty("create_date")]
        public DateTime createdDate { get; set; }
        [JsonProperty("update_date")]
        public DateTime updateDate { get; set; }
        [JsonProperty("registrant")]
        public dynamic registrant { get; set; }
        [JsonProperty("status")]
        public string status { get; set; }
        [JsonProperty("registrar")]
        public dynamic registrar { get; set; }
    }
}
