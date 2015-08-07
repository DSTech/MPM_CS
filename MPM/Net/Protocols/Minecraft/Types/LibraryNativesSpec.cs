using System;
using MPM.Types;

namespace MPM.Net.Protocols.Minecraft.Types {

	/// <summary>
	/// Contains native-library inclusions for each particular operating system, with an optional ${arch} placeholder in circumstances where bitness matters
	/// </summary>
	/// <example>
	/// {
	///  "linux": "natives-linux",
	///  "windows": "natives-windows-${arch}",
	///  "osx": "natives-osx",
	/// }
	/// </example>
	public class LibraryNativesSpec {

		public LibraryNativesSpec(string windows, string linux, string osx) {
			this.Windows = windows ?? "natives-windows";
			this.Linux = linux ?? "natives-linux";
			this.Osx = osx ?? "natives-osx";
		}

		//Returns a string for the natives-name which native variant is to be downloaded for the specified platform,
		//or null if the native variant does not apply
		public string AppliedTo(CompatibilityPlatform platform) {
			switch (platform) {
				case CompatibilityPlatform.Win:
				case CompatibilityPlatform.Win32:
				case CompatibilityPlatform.Win64:
					if (String.IsNullOrWhiteSpace(Windows)) {
						return null;
					}
					if (Windows.Contains("${arch}")) {
						if (platform == CompatibilityPlatform.Win) {
							throw new ArgumentOutOfRangeException(nameof(platform), $"Bitness-inspecific natives are not available for spec {Windows}");
						}
						return Windows.Replace("${arch}", (platform == CompatibilityPlatform.Win32 ? "32" : "64"));
					} else {
						return Windows;
					}
				case CompatibilityPlatform.Lin:
				case CompatibilityPlatform.Lin32:
				case CompatibilityPlatform.Lin64:
					if (String.IsNullOrWhiteSpace(Linux)) {
						return null;
					}
					if (Linux.Contains("${arch}")) {
						if (platform == CompatibilityPlatform.Lin) {
							throw new ArgumentOutOfRangeException(nameof(platform), $"Bitness-inspecific natives are not available for spec {Linux}");
						}
						return Linux.Replace("${arch}", (platform == CompatibilityPlatform.Lin32 ? "32" : "64"));
					} else {
						return Linux;
					}
				//Sorry OSX, make a pull request with support
				default:
					throw new NotSupportedException();
			}
		}

		///<summary>
		/// See info for <seealso cref="LibraryNativesSpec"/>
		///</summary>
		public string Windows { get; }

		///<summary>
		/// See info for <seealso cref="LibraryNativesSpec"/>
		///</summary>
		public string Linux { get; }

		///<summary>
		/// See info for <seealso cref="LibraryNativesSpec"/>
		///</summary>
		public string Osx { get; }
	}
}
