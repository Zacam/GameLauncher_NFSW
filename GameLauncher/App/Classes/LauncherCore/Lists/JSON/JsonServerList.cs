﻿using Newtonsoft.Json;

namespace GameLauncher.App.Classes.LauncherCore.Lists.JSON
{
    /* http://localhost/serverlist.json */
    public class JsonServerList
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("distribution_url")]
        public string DistributionUrl { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ip_address")]
        public string IpAddress { get; set; }

        [JsonProperty("category")]
        public string Category { get; set; }

        [JsonProperty("discord_presence_key")]
        public string DiscordPresenceKey { get; set; }

        [JsonProperty("forceUserAgent")]
        public string ForceUserAgent { get; set; }

        [JsonProperty("discord_application_id")]
        public string DiscordAppId { get; set; }

        [JsonIgnore]
        public bool IsSpecial { get; set; }
    }
}
