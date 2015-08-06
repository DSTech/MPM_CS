using System;
using MPM.Types;

namespace MPM.Net.Protocols.Minecraft.Types {

	public class LibraryRuleFilterOS : LibraryRuleFilter {
		public LibraryRuleFilterOS(string os) {
			this.OS = os.ToLowerInvariant();
		}
		/// <summary>
		/// Checks the filter against the specified platform's OS
		/// </summary>
		/// <param name="platform">The platform to check the filter against</param>
		/// <returns>Whether or not the filter applies to the specified platform</returns>
		public override bool Applies(CompatibilityPlatform platform) {
			switch (OS) {
				case "windows":
					return platform == CompatibilityPlatform.Win || platform == CompatibilityPlatform.Win32 || platform == CompatibilityPlatform.Win64;
				case "linux":
					return platform == CompatibilityPlatform.Lin || platform == CompatibilityPlatform.Lin32 || platform == CompatibilityPlatform.Lin64;
				case "osx":
					return false;
				default:
					throw new NotSupportedException();
			}
		}
		public string OS { get; }
	}
}
