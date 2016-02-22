using System;
using MPM.Types;
using MPM.Util.Json;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class Asset {
        [JsonProperty("path", Required = Required.Always, Order = 1)]
        public Uri @Uri { get; set; }

        [JsonProperty("hash", Required = Required.Always, Order = 2)]
        [JsonConverter(typeof(Sha1HashHexConverter))]
        public Hash @Hash { get; set; }

        [JsonProperty("size", Required = Required.Always, Order = 3)]
        public long Size { get; set; }
    }
}
