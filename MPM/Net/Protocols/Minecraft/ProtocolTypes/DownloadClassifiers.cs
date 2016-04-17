using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class DownloadClassifiers {
        [JsonProperty("natives-linux", NullValueHandling = NullValueHandling.Ignore, Order = 1)]
        public ArtifactSpec Linux { get; set; }

        [JsonProperty("natives-windows", NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public ArtifactSpec Windows { get; set; }

        [JsonProperty("natives-windows-32", NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public ArtifactSpec Windows32 { get; set; }

        [JsonProperty("natives-windows-64", NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public ArtifactSpec Windows64 { get; set; }

        [JsonProperty("natives-osx", NullValueHandling = NullValueHandling.Ignore, Order = 2)]
        public ArtifactSpec Osx { get; set; }
    }
}
