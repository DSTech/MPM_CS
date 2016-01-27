using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SemVer;
using Version = SemVer.Version;

namespace MPM {
    [JsonConverter(typeof(Version))]
    public class VersionConverter : JsonConverter {
        public override bool CanRead {
            get { return true; }
        }

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(Version));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var val = reader.ReadAsString();
            return new Version(val, true);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var val = (Version)value;
            writer.WriteValue(val.ToString());
        }
    }
}
