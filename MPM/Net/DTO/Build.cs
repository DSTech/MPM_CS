using System;

namespace MPM.Net.DTO {

	public class Build {
		public String Package { get; set; }
		public Version Version { get; set; }
		public String GivenVersion { get; set; }
		public String Arch { get; set; }
		public String Platform { get; set; }
		public PackageSide Side { get; set; } = PackageSide.Universal;
		public InterfaceProvision[] Interfaces { get; set; }
		public DependencySpec Dependencies { get; set; }
		public PackageConflict[] Conflicts { get; set; }
		public String[] Hashes { get; set; }
		public bool Stable { get; set; }
		public bool Recommended { get; set; }
	}
}
