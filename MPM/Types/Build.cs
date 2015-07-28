using System;
using System.Collections.Generic;
using System.Linq;
using semver.tools;

namespace MPM.Types {

	public class Build {

		public Build(
			string packageName,
			SemanticVersion version,
			string givenVersion,
			Arch arch,
			CompatibilityPlatform platformCompatibility,
			CompatibilitySide side,
			IEnumerable<InterfaceProvision> interfaceProvisions,
			IEnumerable<InterfaceDependency> interfaceDependencies,
			IEnumerable<PackageDependency> packageDependencies,
			IEnumerable<Conflict> conflicts,
			IEnumerable<Hash> hashes,
			bool stable = false,
			bool recommended = false
		) {
			this.PackageName = packageName;
			this.Version = version;
			this.GivenVersion = givenVersion;
			this.Arch = arch;
			this.Platform = platformCompatibility;
			this.Side = side;
			this.InterfaceProvisions = interfaceProvisions.ToArray();
			this.InterfaceDependencies = interfaceDependencies.ToArray();
			this.PackageDependencies = packageDependencies.ToArray();
			this.Conflicts = conflicts.ToArray();
			this.Hashes = hashes.ToArray();
			this.Stable = stable;
			this.Recommended = recommended;
		}

		public String PackageName { get; }
		public SemanticVersion Version { get; }
		public String GivenVersion { get; }
		public Arch @Arch { get; }
		public CompatibilityPlatform Platform { get; }
		public CompatibilitySide Side { get; }
		public IReadOnlyCollection<InterfaceProvision> InterfaceProvisions { get; }
		public IReadOnlyCollection<InterfaceDependency> InterfaceDependencies { get; }
		public IReadOnlyCollection<PackageDependency> PackageDependencies { get; }
		public IReadOnlyCollection<Conflict> Conflicts { get; }
		public IReadOnlyCollection<Hash> Hashes { get; }
		public bool Stable { get; }
		public bool Recommended { get; }
	}
}
