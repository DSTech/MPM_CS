using System;
using MPM.Core;
using semver.tools;

namespace MPM.Net.DTO {
	public class Build {
		public SemanticVersion Version { get; set; }
		public String GivenVersion { get; set; }
		public InterfaceProvision[] InterfaceProvisions { get; set; }
		public InterfaceDependency[] InterfaceRequirements { get; set; }
		public PackageDependency[] Dependencies { get; set; }
		public PackageConflict[] Conflicts { get; set; }
		public String[] Hashes { get; set; }
		public bool Stable { get; set; }
	}
}
