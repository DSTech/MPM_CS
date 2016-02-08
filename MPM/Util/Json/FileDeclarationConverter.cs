using System;
using System.Linq;
using MPM.Core.Instances.Info;
using MPM.Core.Instances.Installation.Scripts;
using Newtonsoft.Json;

namespace MPM.Util.Json {
    public class FileDeclarationConverter : JsonConverter {
        public override bool CanRead {
            get { return true; }
        }

        public override bool CanConvert(Type objectType) {
            return (objectType == typeof(IFileDeclaration));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var scriptDecl = serializer.Deserialize<ScriptFileDeclaration>(reader);
            return scriptDecl.Parse(null, null);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var val = (IFileDeclaration)value;
            var scriptDecl = new ScriptFileDeclaration {
                Description = val.Description,
                Hash = val.Hash,
                Source = val.Source,
                Targets = val.Targets?.ToArray(),
            };
            var sourcelessDecl = val as SourcelessFileDeclaration;
            if (sourcelessDecl != null) {
                scriptDecl.Type = sourcelessDecl.Type;
            }
            serializer.Serialize(writer, scriptDecl, typeof(ScriptFileDeclaration));
        }
    }
}
