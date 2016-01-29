using System;
using MPM.Types;
using Newtonsoft.Json;

namespace MPM.Util.Json {
    [JsonConverter(typeof(Hash))]
    public class HashConverter : JsonConverter {
        public override bool CanRead {
            get { return true; }
        }

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(Hash));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return Hash.Parse(serializer.Deserialize<string>(reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var val = (Hash)value;
            writer.WriteValue(val.ToString());
        }
    }
}
