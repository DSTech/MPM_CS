using System;
using MPM.Core;
using semver.tools;

namespace MPM.Net.DTO {
	public class DependencySpec {
		public PackageDependency[] Packages { get; set; } = new PackageDependency[0];
		public InterfaceDependency[] Interfaces { get; set; } = new InterfaceDependency[0];
	}
}
