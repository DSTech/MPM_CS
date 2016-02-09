using System;
using System.Linq;
using MPM.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Platform;

namespace MPM.Util.Json {
    public class Sha1HashHexConverter : JsonConverter {
        public override bool CanRead {
            get { return true; }
        }

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(Hash));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return new Hash("sha1", Hex.GetBytes(serializer.Deserialize<string>(reader)));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var val = (Hash)value;
            writer.WriteValue(Hex.GetString(val.Checksum).ToLower());
        }
    }
}
