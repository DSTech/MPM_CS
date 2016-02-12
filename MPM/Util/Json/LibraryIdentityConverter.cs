using System;
using System.Linq;
using MPM.Core.Instances.Info;
using MPM.Core.Instances.Installation.Scripts;
using MPM.Net.Protocols.Minecraft.ProtocolTypes;
using Newtonsoft.Json;

namespace MPM.Util.Json {
    public class LibraryIdentityConverter : JsonConverter {
        public override bool CanRead {
            get { return true; }
        }

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(LibraryIdentity));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            return LibraryIdentity.FromString(serializer.Deserialize<String>(reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var val = ((LibraryIdentity)value).ToString();
            writer.WriteValue(val);
        }
    }
}
