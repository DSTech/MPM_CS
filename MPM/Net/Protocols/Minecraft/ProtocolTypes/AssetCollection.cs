using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class AssetCollection {
        [JsonProperty("objects", Required = Required.Always)]
        public Dictionary<Uri, Asset> Assets { get; set; }
    }
}
