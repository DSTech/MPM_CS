using System;
using System.Collections.Generic;
using System.Linq;
using semver.tools;

namespace MPM.Types {
    public class Build : IEquatable<Build> {
        public Build() {
        }

        public Build(
            string packageName,
            IEnumerable<Author> authors,
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
            this.Authors = authors.ToList();
            this.Version = version;
            this.GivenVersion = givenVersion;
            this.Arch = arch;
            this.Platform = platformCompatibility;
            this.Side = side;
            this.InterfaceProvisions = interfaceProvisions.AsEnumerable().ToList();
            this.InterfaceDependencies = interfaceDependencies.AsEnumerable().ToList();
            this.PackageDependencies = packageDependencies.AsEnumerable().ToList();
            this.Conflicts = conflicts.AsEnumerable().ToList();
            this.Hashes = hashes.AsEnumerable().ToList();
            this.Stable = stable;
            this.Recommended = recommended;
        }

        public String PackageName { get; set; }
        public List<Author> Authors { get; set; }
        public SemanticVersion Version { get; set; }
        public String GivenVersion { get; set; }
        public Arch @Arch { get; set; }
        public CompatibilityPlatform Platform { get; set; }
        public CompatibilitySide Side { get; set; }
        public List<InterfaceProvision> InterfaceProvisions { get; set; }
        public List<InterfaceDependency> InterfaceDependencies { get; set; }
        public List<PackageDependency> PackageDependencies { get; set; }
        public List<Conflict> Conflicts { get; set; }
        public List<Hash> Hashes { get; set; }
        public bool Stable { get; set; }
        public bool Recommended { get; set; }

        #region Equality members

        public bool Equals(Build other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return string.Equals(this.PackageName, other.PackageName) &&
                Equals(this.Authors, other.Authors) && Equals(this.Version, other.Version) &&
                string.Equals(this.GivenVersion, other.GivenVersion) && Equals(this.Arch, other.Arch) &&
                this.Platform == other.Platform && this.Side == other.Side &&
                Equals(this.InterfaceProvisions, other.InterfaceProvisions) &&
                Equals(this.InterfaceDependencies, other.InterfaceDependencies) &&
                Equals(this.PackageDependencies, other.PackageDependencies) && Equals(this.Conflicts, other.Conflicts) &&
                Equals(this.Hashes, other.Hashes) && this.Stable == other.Stable && this.Recommended == other.Recommended;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as Build;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = this.PackageName != null ? this.PackageName.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (this.Authors != null ? this.Authors.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Version != null ? this.Version.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.GivenVersion != null ? this.GivenVersion.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Arch != null ? this.Arch.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (int)this.Platform;
                hashCode = (hashCode * 397) ^ (int)this.Side;
                hashCode = (hashCode * 397) ^ (this.InterfaceProvisions != null ? this.InterfaceProvisions.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.InterfaceDependencies != null ? this.InterfaceDependencies.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.PackageDependencies != null ? this.PackageDependencies.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Conflicts != null ? this.Conflicts.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Hashes != null ? this.Hashes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.Stable.GetHashCode();
                hashCode = (hashCode * 397) ^ this.Recommended.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(Build left, Build right) {
            return Equals(left, right);
        }

        public static bool operator !=(Build left, Build right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
