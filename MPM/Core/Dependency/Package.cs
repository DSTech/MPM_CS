using System;
using semver.tools;

namespace MPM.Core.Dependency {
	public class PackageSpec {
		public String Name { get; set; }
		public SemanticVersion Version { get; set; }
		public bool Manual { get; set; }
	}
}
