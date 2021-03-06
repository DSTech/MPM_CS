using System;
using MPM.Types;
using Newtonsoft.Json;

namespace MPM.Util.Json {
    public class ArchConverter : JsonConverter {
        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(Arch));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return new Arch(serializer.Deserialize<string>(reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var val = (Arch)value;
            serializer.Serialize(writer, val?.Id);
        }
    }
}
