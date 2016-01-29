using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace MPM.Core.Instances.Installation.Scripts {
    [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum SourcelessType {
        [EnumMember(Value = "configuration")] Configuration,
        [EnumMember(Value = "cache")] Cache,
    }
}
