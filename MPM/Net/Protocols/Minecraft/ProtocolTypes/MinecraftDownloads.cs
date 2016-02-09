using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class MinecraftDownloads {
        [JsonProperty("client")]
        public MinecraftDownload Client { get; set; }

        [JsonProperty("server")]
        public MinecraftDownload Server { get; set; }

        [JsonProperty("windows_server")]
        public MinecraftDownload WindowsServer { get; set; }
    }
}
