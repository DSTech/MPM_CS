using System;
using semver.tools;

namespace MPM.Types {

	public class InterfaceProvision {

		public InterfaceProvision(string interfaceName, SemanticVersion version) {
			this.InterfaceName = interfaceName;
			this.Version = version;
		}

		public String InterfaceName { get; }
		public SemanticVersion Version { get; }
	}
}
