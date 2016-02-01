using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MPM.CLI {
    [JsonConverter(typeof(StringEnumConverter))]
    public enum InstanceSide {
        [EnumMember(Value = "client")]
        Client,
        [EnumMember(Value = "server")]
        Server,
    }
}
