using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MPM.Core.Instances.Info;
using MPM.Data;
using MPM.Data.Repository;
using MPM.Types;

namespace MPM.Core.Dependency {
    public static class PackageSpecExtensions {
        public static PackageSpec ToSpec(this PackageDependency dependency, Arch arch, bool manual = false) {
            var spec = new PackageSpec {
                Manual = manual,
                Arch = arch,
                Name = dependency.PackageName,
                Side = dependency.Side,
                VersionSpec = dependency.VersionSpec,
            };
            return spec;
        }

        /// <summary>
        ///     Checks if a build satisfies the given specification.
        /// </summary>
        /// <param name="spec">Specification to check against</param>
        /// <param name="build">Build to check. Assumes (What?)</param>
        /// <returns></returns>
        public static bool Satisfies(this PackageSpec spec, Build build) {
            return spec.Name == build.PackageName
                && spec.Arch == build.Arch
                && (
                    spec.Side == build.Side || build.Side == CompatibilitySide.Universal
                    )
                && spec.VersionSpec.IsSatisfied(build.Version);
        }

        /// <summary>
        ///     Looks up a package, returning named builds qualifying for the specification.
        ///     Must return in descending order of version.
        /// </summary>
        /// <param name="repository">Repository to look up the spec from within</param>
        /// <param name="packageSpec">Specification to look up</param>
        /// <returns>Builds in descending order of version</returns>
        /// <remarks>Should be converted to return IQueryable to allow optimized behavior with constraint lookup</remarks>
        public static IEnumerable<Build> LookupSpec([NotNull] this IPackageRepository repository, PackageSpec packageSpec) {
            var builds = repository.FetchBuilds().ToArray();
            return builds
                .Where(packageSpec.Satisfies)
                .OrderByDescending(b => b.Version)
                .ThenByDescending(b => b.Side != CompatibilitySide.Universal)
                .ToArray();
        }
    }

    public class PackageSpec : IEquatable<PackageSpec> {
        public String Name { get; set; }
        public Arch Arch { get; set; }
        public MPM.Types.SemRange @VersionSpec { get; set; }
        public CompatibilitySide Side { get; set; } = CompatibilitySide.Universal;
        public bool Manual { get; set; } = false;

        public bool Equals(PackageSpec other) {
            return
                Name == other.Name
                    && this.Arch == other.Arch
                    && this.VersionSpec.ToString() == other.VersionSpec.ToString()
                    && this.Manual == other.Manual;
        }

        public override bool Equals(object obj) {
            var packageSpec = obj as PackageSpec;
            if (packageSpec == null) {
                return obj == null;
            }
            return Equals(packageSpec);
        }

        public override int GetHashCode() {
            return
                (Name?.GetHashCode() ?? 0)
                    + (Arch?.GetHashCode() ?? 0)
                    + (VersionSpec?.GetHashCode() ?? 0)
                    + Manual.GetHashCode();
        }
        public override string ToString() {
            return $"Specification: \"{Name}\" for {Arch} with version {VersionSpec} for side {Side} (MANUAL:{Manual})";
        }
    }
}
