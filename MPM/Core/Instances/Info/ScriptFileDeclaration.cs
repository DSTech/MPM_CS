using System;
using MPM.Core.Instances.Installation.Scripts;
using MPM.Types;
using Newtonsoft.Json;

namespace MPM.Core.Instances.Info {
    public class ScriptFileDeclaration {
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public SourcelessType? Type { get; set; }
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public String Description { get; set; }
        [JsonProperty("source", NullValueHandling = NullValueHandling.Ignore)]
        public String Source { get; set; }
        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public Hash @Hash { get; set; }
        [JsonProperty("targets", NullValueHandling = NullValueHandling.Ignore)]
        public String[] Targets { get; set; }
    }
}
