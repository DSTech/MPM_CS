using System;
using System.Linq;
using MPM.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Platform;

namespace MPM.Util.Json {
    public class HashConverter : JsonConverter {
        public override bool CanRead {
            get { return true; }
        }

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(Hash));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            if (reader.TokenType != JsonToken.String) {
                return readOldHashFormat(reader, objectType, existingValue, serializer);
            }
            return Hash.Parse(serializer.Deserialize<string>(reader));
        }

        private static Hash readOldHashFormat(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var oldObj = JObject.Load(reader);
            var algorithm = "";
            var checksum = "";
            foreach (var prop in oldObj.Properties()) {
                switch (prop.Name.Trim().ToLowerInvariant()) {
                    case "algorithm":
                        algorithm = prop.Value.ToString();
                        break;
                    case "checksum":
                        checksum = prop.Value.ToString();
                        break;
                }
            }
            if (algorithm.IsNullOrEmpty() || checksum.IsNullOrEmpty()) {
                throw new FormatException("Could not recognize hash format");
            }
            return new Hash(algorithm, Base64.GetBytesUnknown(checksum));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var val = (Hash)value;
            writer.WriteValue(val.ToString());
        }
    }
}
