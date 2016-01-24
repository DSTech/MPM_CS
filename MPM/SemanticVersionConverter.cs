using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using semver.tools;

namespace MPM {
    public class SemanticVersionConverter : JsonConverter {
        public override bool CanRead {
            get { return true; }
        }

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(SemanticVersion));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var tok = JToken.Load(reader);
            var tokStr = tok.ToString();
            return SemanticVersion.ParseNuGet(tokStr);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var val = (SemanticVersion) value;
            writer.WriteValue(val.ToString());
        }
    }
}
