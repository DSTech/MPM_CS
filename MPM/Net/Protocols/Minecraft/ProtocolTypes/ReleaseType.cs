using System.Runtime.Serialization;
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
