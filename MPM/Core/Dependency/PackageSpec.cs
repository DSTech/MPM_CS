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

        public override string ToString() {
            return $"Specification: \"{Name}\" for {Arch} with version {VersionSpec} for side {Side} (MANUAL:{Manual})";
        }

        #region Equality members

        public bool Equals(PackageSpec other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return string.Equals(this.Name, other.Name)
                && Equals(this.Arch, other.Arch)
                && Equals(this.VersionSpec, other.VersionSpec)
                && this.Side == other.Side
                && this.Manual == other.Manual;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != this.GetType()) { return false; }
            return Equals((PackageSpec)obj);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (this.Name != null ? this.Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Arch != null ? this.Arch.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.VersionSpec != null ? this.VersionSpec.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)this.Side;
                hashCode = (hashCode * 397) ^ this.Manual.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(PackageSpec left, PackageSpec right) {
            return Equals(left, right);
        }

        public static bool operator !=(PackageSpec left, PackageSpec right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
