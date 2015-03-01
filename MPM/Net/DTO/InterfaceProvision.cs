using System;
using MPM.Core;
using semver.tools;

namespace MPM.Net.DTO {
	public class InterfaceProvision {
		public String Name { get; set; }
		public SemanticVersion Version { get; set; }
	}
}
