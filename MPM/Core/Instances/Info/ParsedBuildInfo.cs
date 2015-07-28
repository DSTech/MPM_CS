using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Dependency;
using MPM.Core.Instances.Installation.Scripts;
using MPM.Types;
using semver.tools;

namespace MPM.Core.Instances.Info {

	public class ParsedBuildInfo : Build {

		public ParsedBuildInfo(
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
			IEnumerable<IFileDeclaration> installation,
			bool stable = false,
			bool recommended = false
		) : base(
			packageName,
			version,
			givenVersion,
			arch,
			platformCompatibility,
			side,
			interfaceProvisions,
			interfaceDependencies,
			packageDependencies,
			conflicts,
			hashes,
			stable,
			recommended
		) {
			this.Installation = installation.ToArray();
		}

		public ParsedBuildInfo(
			Build build,
			IEnumerable<IFileDeclaration> installation
		) : base(
			build.PackageName,
			build.Version,
			build.GivenVersion,
			build.Arch,
			build.Platform,
			build.Side,
			build.InterfaceProvisions,
			build.InterfaceDependencies,
			build.PackageDependencies,
			build.Conflicts,
			build.Hashes,
			build.Stable,
			build.Recommended
		) {
			this.Installation = installation.ToArray();
		}

		public IReadOnlyList<IFileDeclaration> Installation { get; set; }
	}
}
