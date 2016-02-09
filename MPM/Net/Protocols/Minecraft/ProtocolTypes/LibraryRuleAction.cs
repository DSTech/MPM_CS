using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum LibraryRuleAction {
        [EnumMember(Value = "disallow")]
        Disallow,
        [EnumMember(Value = "allow")]
        Allow,
    }
}
