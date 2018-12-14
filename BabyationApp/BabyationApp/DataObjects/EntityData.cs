using System;
using Microsoft.WindowsAzure.MobileServices;
using Newtonsoft.Json;

namespace BabyationApp.DataObjects
{


  
        public class EntityData
        {
            [JsonProperty(PropertyName = "createdAt")]
            public DateTimeOffset CreatedAt { get; set; }
            [JsonProperty(PropertyName = "id")]
            public string Id { get; set; }
            [UpdatedAt]
            public DateTimeOffset UpdatedAt { get; set; }
            [Version]
            public string Version { get; set; }
        }
    }
