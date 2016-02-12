using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Util.Json;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    [JsonConverter(typeof(LibraryIdentityConverter))]
    public struct LibraryIdentity {
        public LibraryIdentity(string package, string name, string version) {
            this.Package = package;
            this.Name = name;
            this.Version = version;
        }

        [JsonIgnore]
        public string Package { get; set; }

        [JsonIgnore]
        public string Name { get; set; }

        [JsonIgnore]
        public string Version { get; set; }

        public static LibraryIdentity FromString(string identityString) {
            return ParseIdentity(identityString);
        }

        private static LibraryIdentity ParseIdentity(string identityString) {
            var pairs = identityString.Split(new[] { ':' }, 3);
            return new LibraryIdentity {
                Package = pairs[0]
            };
        }

        public override string ToString() => $"{Package}:{Name}:{Version}";
    }
}
