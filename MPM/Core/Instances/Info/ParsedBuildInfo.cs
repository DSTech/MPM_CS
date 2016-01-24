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
            IEnumerable<Author> authors,
            SemanticVersion version,
            string givenVersion,
            Arch arch,
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
            authors,
            version,
            givenVersion,
            arch,
            side,
            interfaceProvisions,
            interfaceDependencies,
            packageDependencies,
            conflicts,
            hashes
        ) {
            this.Installation = installation.ToArray();
        }

        public ParsedBuildInfo(
            Build build,
            IEnumerable<IFileDeclaration> installation
        ) : base(
            build.PackageName,
            build.Authors,
            build.Version,
            build.GivenVersion,
            build.Arch,
            build.Side,
            build.InterfaceProvisions,
            build.InterfaceDependencies,
            build.PackageDependencies,
            build.Conflicts,
            build.Hashes
        ) {
            this.Installation = installation.ToArray();
        }

        public IReadOnlyList<IFileDeclaration> Installation { get; set; }
    }
}
