using System;
using MPM.Core;

namespace MPM.Net.DTO {
	public class PackageDependency {
		public string Name { get; set; }
		public VersionSpecification Version { get; set; }
	}
}
