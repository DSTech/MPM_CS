using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SemVer;
using Version = SemVer.Version;

namespace MPM {
    public class VersionRangeConverter : JsonConverter {
        public override bool CanRead {
            get { return true; }
        }

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(Range));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var val = reader.ReadAsString();
            return new Range(val, loose: true);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var val = (Range) value;
            writer.WriteValue(val.ToString());
        }
    }
}
