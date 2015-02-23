using System;
using System.Collections.Generic;

namespace MPM.Core {
	public class Build {
		public VersionIdentifier Version { get; set; }
		public string GivenVersion { get; set; }
		public InterfaceProvision[] InterfaceProvisions { get; set; }
		public InterfaceDependency[] InterfaceRequirements { get; set; }
		public PackageDependency[] Dependencies { get; set; }
		public PackageConflict[] Conflicts { get; set; }
		public bool Recommended { get; set; }
	}
}
