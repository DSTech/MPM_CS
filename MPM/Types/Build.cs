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
            CompatibilitySide side,
            IEnumerable<InterfaceProvision> interfaceProvisions,
            IEnumerable<InterfaceDependency> interfaceDependencies,
            IEnumerable<PackageDependency> packageDependencies,
            IEnumerable<Conflict> conflicts,
            IEnumerable<Hash> hashes
            ) {
            this.PackageName = packageName;
            this.Authors = authors.ToList();
            this.Version = version;
            this.GivenVersion = givenVersion;
            this.Arch = arch;
            this.Side = side;
            this.InterfaceProvisions = interfaceProvisions.AsEnumerable().ToList();
            this.InterfaceDependencies = interfaceDependencies.AsEnumerable().ToList();
            this.PackageDependencies = packageDependencies.AsEnumerable().ToList();
            this.Conflicts = conflicts.AsEnumerable().ToList();
            this.Hashes = hashes.AsEnumerable().ToList();
        }

        public String PackageName { get; set; }
        public List<Author> Authors { get; set; }
        public SemanticVersion Version { get; set; }
        public String GivenVersion { get; set; }
        public Arch @Arch { get; set; }
        public CompatibilitySide Side { get; set; }
        public List<InterfaceProvision> InterfaceProvisions { get; set; }
        public List<InterfaceDependency> InterfaceDependencies { get; set; }
        public List<PackageDependency> PackageDependencies { get; set; }
        public List<Conflict> Conflicts { get; set; }
        public List<Hash> Hashes { get; set; }

        #region Equality members

        public bool Equals(Build other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return string.Equals(this.PackageName, other.PackageName, StringComparison.InvariantCultureIgnoreCase)
                && Equals(this.Authors, other.Authors)
                && Equals(this.Version, other.Version)
                && string.Equals(this.GivenVersion, other.GivenVersion, StringComparison.InvariantCultureIgnoreCase)
                && Equals(this.Arch, other.Arch)
                && this.Side == other.Side
                && Equals(this.InterfaceProvisions, other.InterfaceProvisions)
                && Equals(this.InterfaceDependencies, other.InterfaceDependencies)
                && Equals(this.PackageDependencies, other.PackageDependencies)
                && Equals(this.Conflicts, other.Conflicts)
                && Equals(this.Hashes, other.Hashes);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            var other = obj as Build;
            return other != null && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (this.PackageName != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(this.PackageName) : 0);
                hashCode = (hashCode * 397) ^ (this.Authors?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Version?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.GivenVersion != null ? StringComparer.InvariantCultureIgnoreCase.GetHashCode(this.GivenVersion) : 0);
                hashCode = (hashCode * 397) ^ (this.Arch?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (int) this.Side;
                hashCode = (hashCode * 397) ^ (this.InterfaceProvisions?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.InterfaceDependencies?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.PackageDependencies?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Conflicts?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Hashes?.GetHashCode() ?? 0);
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

        #region Equality members

        #endregion
    }
}
