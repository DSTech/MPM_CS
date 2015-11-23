using System;
using MPM.Types;

namespace MPM.Net.DTO {

	public class Version : IEquatable<Version> {

		public bool Equals(Version other) {
			if (other == null) return false;
			return (this.Major.Equals(other.Major) && this.Minor.Equals(other.Minor) && this.Patch.Equals(other.Patch));
		}

		public override bool Equals(object obj) => Equals(obj as Version);

		public override int GetHashCode() {
			return new Tuple<UInt16, UInt16, UInt16>(Major, Minor, Patch).GetHashCode();
		}

		public static bool operator ==(Version first, Version second) {
			if (object.ReferenceEquals(first, second)) return true;
			if (object.ReferenceEquals(first, null)) return false;
			if (object.ReferenceEquals(second, null)) return false;

			return first.Equals(second);
		}

		public static bool operator !=(Version first, Version second) {
			if (object.ReferenceEquals(first, second)) return false;
			if (object.ReferenceEquals(first, null)) return true;
			if (object.ReferenceEquals(second, null)) return true;

			return !first.Equals(second);
		}

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
