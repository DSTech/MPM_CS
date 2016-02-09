using System.Collections.Generic;
using System.Linq;
using MPM.Extensions;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class LibraryExtractSpec {
        public LibraryExtractSpec() {
        }

        public LibraryExtractSpec(IEnumerable<string> exclusionPaths = null) {
            Exclusions = exclusionPaths.Denull().ToList();
        }

        /// <summary>
        ///     A list of paths to exclude from extraction when installing the library
        /// </summary>
        [JsonProperty("exclude")]
        public List<string> Exclusions { get; set; }
    }
}
