using System;

namespace MPM.Net.DTO {

	public class DependencySpec {
		public PackageDependency[] Packages { get; set; }
		public InterfaceDependency[] Interfaces { get; set; }
	}
}
