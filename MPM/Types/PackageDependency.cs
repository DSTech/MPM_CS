using System;
using semver.tools;

namespace MPM.Types {

	public class PackageDependency {

		public PackageDependency(string packageName, VersionSpec versionSpec, CompatibilitySide side) {
			this.PackageName = packageName;
			this.VersionSpec = versionSpec;
			this.Side = side;
		}

		public String PackageName { get; }
		public VersionSpec @VersionSpec { get; }
		public CompatibilitySide Side { get; }
	}
}
