using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitsimpBot.DataContext
{
    
    public class Friend
    {

        [JsonProperty("id")]
        public string ID { get; set; }
        [JsonProperty("displayName")]
        public string displayName { get; set; }
#nullable enable
        [JsonProperty("bio")]
        public string? bio { get; set; }
        [JsonProperty("currentAvatarImageUrl")]
        public string? currentAvatarImageUrl { get; set; }
        [JsonProperty("currentAvatarThumbnailImageUrl")]
        public string? currentAvatarThumbnailImageUrl { get; set; }
        [JsonProperty("fallbackAvatar")]
        public string? fallbackAvatar { get; set; }
        [JsonProperty("userIcon")]
        public string? userIcon { get; set; }
        [JsonProperty("last_platform")]
        public string? last_platform { get; set; }
        [JsonProperty("tags")]
        public List<Object>? tags { get; set; }
        [JsonProperty("developerType")]
        public string? developerType { get; set; }
        [JsonProperty("status")]
        public string? status { get; set; }
        [JsonProperty("statusDescription")]
        public string? statusDescription { get; set; }
        [JsonProperty("friendKey")]
        public string? friendKey { get; set; }
        [JsonProperty("last_login")]
        public DateTime? last_login { get; set; }
        [JsonProperty("isFriend")]
        public bool? isFriend { get; set; }
        [JsonProperty("location")]
        public string? location { get; set; }

    }
}
