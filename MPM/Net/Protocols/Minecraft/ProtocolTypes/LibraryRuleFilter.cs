using System;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public abstract class LibraryRuleFilter {
        /// <summary>
        ///     Checks the filter against the specified platform
        /// </summary>
        /// <param name="platform">The platform to check the filter against</param>
        /// <returns>Whether or not the filter applies to the specified platform</returns>
        public abstract bool Applies(PlatformID platform, bool x64 = true);
    }
}
