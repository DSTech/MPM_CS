using System;
using MPM.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MPM.Util.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class ArtifactSpec {
        [JsonProperty("size", Order = 1)]
        public long Size { get; set; }

        [JsonProperty("sha1", Order = 2)]
        [JsonConverter(typeof(Sha1HashHexConverter))]
        public Hash Hash;

        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public string Path { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore, Order = 4)]
        public string Url { get; set; }
    }
}