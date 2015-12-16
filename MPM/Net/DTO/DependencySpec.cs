using System.Collections.Generic;

namespace MPM.Net.DTO {
	public class DependencySpec {
		public List<PackageDependency> Packages { get; set; }
		public List<InterfaceDependency> Interfaces { get; set; }
	}
}
