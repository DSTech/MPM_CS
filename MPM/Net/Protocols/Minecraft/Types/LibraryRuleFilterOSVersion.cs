using System;
using MPM.Types;

namespace MPM.Net.Protocols.Minecraft.Types {
    public class LibraryRuleFilterOSVersion : LibraryRuleFilter {
        public LibraryRuleFilterOSVersion() {
        }

        public LibraryRuleFilterOSVersion(string version) {
            this.Version = version.ToLowerInvariant();
        }

        public string Version { get; set; }

        /// <summary>
        ///     Checks the filter against the specified platform's OS version
        /// </summary>
        /// <param name="platform">The platform to check the filter against</param>
        /// <returns>Whether or not the filter applies to the specified platform</returns>
        public override bool Applies(PlatformID platform, bool x64 = true) {
            //See http://wiki.vg/Game_Files#Libraries
            //TODO: OS versions are unaccounted for, for now. They are not part of the MPM specification.
            return true;
        }
    }
}
