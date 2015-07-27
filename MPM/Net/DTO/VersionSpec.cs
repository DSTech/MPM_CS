using System;
using System.Linq;

namespace MPM.Net.DTO {

	public class VersionSpec {
		public Version Minimum { get; set; }
		public bool MinInclusive { get; set; }
		public Version Maximum { get; set; }
		public bool MaxInclusive { get; set; }

		public static VersionSpec Parse(string versionString) {
			var _versionString = versionString.Trim();
			var leftIncluder = _versionString.First();
			var rightIncluder = _versionString.Last();
			var range = new String(_versionString.Skip(1).SkipLast(1).ToArray()).Trim();

			var splittedRange = range.Split(new[] { ',' }, 2);

			if (splittedRange.Length != 2 || (leftIncluder != '[' && leftIncluder != '(') || (rightIncluder != ']' && rightIncluder != ')')) {
				throw new FormatException("VersionSpec string did not match the correct formatting");
			}
			var minVersion = Version.Parse(splittedRange[0]);
			var minInclusive = leftIncluder == '[';
			var maxVersion = Version.Parse(splittedRange[1]);
			var maxInclusive = rightIncluder == ']';

			return new VersionSpec {
				Minimum = minVersion,
				MinInclusive = minInclusive,
				Maximum = maxVersion,
				MaxInclusive = maxInclusive,
			};
		}

		public override string ToString() {
			return $"{(MinInclusive ? '[' : '(')}{Minimum},{Maximum}{(MaxInclusive ? ']' : ')')}";
		}
	}
}
