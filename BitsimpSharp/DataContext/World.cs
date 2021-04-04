using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitsimpBot.DataContext
{
    
    public class World
    {
        [JsonProperty("id")]
        public string WorldID {get;set;}
        [JsonProperty("name")]
        public string WorldName {get;set;}
        [JsonProperty("authorId")]
        public string AuthorID {get;set;}
        [JsonProperty("authorName")]
        public string AuthorName {get;set;}
        [JsonProperty("capacity")]
        public int MaxPlayers {get;set;}
        [JsonProperty("thumbnailImageUrl")]
        public string WorldImage {get;set;}
        [JsonProperty("created_at")]
        public DateTime DateCreated {get;set;}
        
    }
}
