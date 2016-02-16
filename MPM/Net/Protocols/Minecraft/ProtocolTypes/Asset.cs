using MPM.Types;
using MPM.Util.Json;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class Asset {
        [JsonProperty("hash", Required = Required.Always, Order = 1)]
        [JsonConverter(typeof(Sha1HashHexConverter))]
        public Hash @Hash { get; set; }

        [JsonProperty("size", Required = Required.Always, Order = 2)]
        public long Size { get; set; }
    }
}
