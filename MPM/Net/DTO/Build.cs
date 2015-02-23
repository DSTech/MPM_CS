using System;
using System.Collections.Generic;
using MPM.Core;

namespace MPM.Net.DTO {
	public class Build {
		public string PackageName { get; set; }
		public VersionIdentifier Version { get; set; }
		public string GivenVersion { get; set; }
		public InterfaceProvision[] InterfaceProvisions { get; set; }
		public InterfaceDependency[] InterfaceRequirements { get; set; }
		public PackageDependency[] Dependencies { get; set; }
		public PackageConflict[] Conflicts { get; set; }
	}
}
