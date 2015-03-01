using System;
using MPM.Core;
using semver.tools;

namespace MPM.Net.DTO {
	public class InterfaceDependency {
		public String Name { get; set; }
		public VersionSpec Version { get; set; }
	}
}
