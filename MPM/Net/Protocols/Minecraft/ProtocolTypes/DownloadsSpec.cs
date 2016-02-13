using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using MPM.Util.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class DownloadsSpec {
        [JsonProperty("classifiers", NullValueHandling = NullValueHandling.Ignore, Order = 1)]
        public DownloadClassifiers Classifiers { get; set; }

        [JsonProperty("artifact", NullValueHandling = NullValueHandling.Ignore, Order = 2)]
        public ArtifactSpec Artifact { get; set; }
    }
}