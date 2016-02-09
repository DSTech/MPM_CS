using MPM.Types;
using MPM.Util.Json;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class MinecraftDownload {
        [JsonProperty("sha1")]
        [JsonConverter(typeof(Sha1HashHexConverter))]
        public Hash Sha1 { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
