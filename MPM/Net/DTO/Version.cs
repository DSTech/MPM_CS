using System;

namespace MPM.Net.DTO {

	public class Version {
		public UInt16 Major { get; set; }
		public UInt16 Minor { get; set; }
		public UInt16 Patch { get; set; }

		public static Version Parse(string versionString) {
			var splitted = versionString.Trim().Split(new[] { '.' }, 3);
			if (splitted.Length != 3) {
				throw new FormatException("Version string did not match the correct formatting");
			}
			return new Version {
				Major = UInt16.Parse(splitted[0]),
				Minor = UInt16.Parse(splitted[1]),
				Patch = UInt16.Parse(splitted[2]),
			};
		}

		public override string ToString() {
			return $"{Major}.{Minor}.{Patch}";
		}
	}
}
