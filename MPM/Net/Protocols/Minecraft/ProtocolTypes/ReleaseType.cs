using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum ReleaseType {
        [EnumMember(Value = "release")]
        Release,
        [EnumMember(Value = "snapshot")]
        Snapshot,
        [EnumMember(Value = "old_alpha")]
        OldAlpha,
        [EnumMember(Value = "old_beta")]
        OldBeta,
    }
}
