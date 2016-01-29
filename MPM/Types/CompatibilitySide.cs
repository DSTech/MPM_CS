using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MPM.Types {
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum CompatibilitySide {
        [EnumMember(Value = "client")] Client,
        [EnumMember(Value = "server")] Server,
        [EnumMember(Value = "universal")] Universal,
    }
}
