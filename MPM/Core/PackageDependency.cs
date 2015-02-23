using System;

namespace MPM.Core {
	public class PackageDependency {
		public string Name { get; set; }
		public VersionSpecification Version { get; set; }
	}
}
