using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MPM.Types;

namespace MPM.Util.Json {
    public class VersionRangeConverter : JsonConverter {
        public bool Loose { get; set; } = true;

        public VersionRangeConverter() {
        }

        public VersionRangeConverter(bool loose) {
            this.Loose = loose;
        }

        public override bool CanRead {
            get { return true; }
        }

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(SemRange));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return new SemRange(serializer.Deserialize<string>(reader), loose: Loose);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var val = (SemRange)value;
            writer.WriteValue(val.ToString());
        }
    }
}
