using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MPM.Types;
using SemVersion = MPM.Types.SemVersion;

namespace MPM.Util.Json {
    public class VersionConverter : JsonConverter {
        public override bool CanRead {
            get { return true; }
        }

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(SemVersion));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return new SemVersion(serializer.Deserialize<string>(reader), true);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var val = (SemVersion)value;
            writer.WriteValue(val.ToString());
        }
    }
}
