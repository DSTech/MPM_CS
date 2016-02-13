using System;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class LibraryRuleFilterOS : LibraryRuleFilter {
        public LibraryRuleFilterOS() {
        }

        public LibraryRuleFilterOS(string os) {
            this.OS = os.ToLowerInvariant();
        }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string OS { get; set; }

        /// <summary>
        ///     Checks the filter against the specified platform's OS
        /// </summary>
        /// <param name="platform">The platform to check the filter against</param>
        /// <param name="x64">The bitness to check against</param>
        /// <returns>Whether or not the filter applies to the specified platform</returns>
        public override bool Applies(PlatformID platform, bool x64 = true) {
            switch (OS) {
                case "windows":
                    return platform == PlatformID.Win32NT;
                case "linux":
                    return platform == PlatformID.Unix;
                case "osx":
                    return platform == PlatformID.MacOSX;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
