using System;
using semver.tools;

namespace MPM.Types {

	public class InterfaceDependency {

		public InterfaceDependency(string interfaceName, VersionSpec versionSpec, CompatibilitySide side) {
			this.InterfaceName = interfaceName;
			this.VersionSpec = versionSpec;
			this.Side = side;
		}

		public String InterfaceName { get; }
		public VersionSpec @VersionSpec { get; }
		public CompatibilitySide Side { get; }
	}
}
