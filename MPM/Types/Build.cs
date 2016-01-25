using System;
using System.Collections.Generic;
using System.Linq;
using MPM.Core.Instances.Info;
using MPM.Extensions;
using Newtonsoft.Json;
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
            IEnumerable<Hash> hashes = null,
            IEnumerable<ScriptFileDeclaration> installation = null
            ) {
            this.PackageName = packageName;
            this.Authors = authors.ToList();
            this.Version = version;
            this.GivenVersion = givenVersion;
            this.Arch = arch;
            this.Side = side;
            this.Interfaces = interfaceProvisions.Denull().ToList();
            this.Dependencies = new BuildDependencySet {
                Interfaces = interfaceDependencies.Denull().ToList(),
                Packages = packageDependencies.Denull().ToList(),
            };
            this.Conflicts = conflicts.Denull().ToList();
            this.Hashes = hashes?.ToList();
            this.Installation = installation?.ToList();
        }

        public Build Clone() {
            return new Build(
                this.PackageName,
                this.Authors,
                this.Version,
                this.GivenVersion,
                this.@Arch,
                this.Side,
                this.Interfaces,
                this.Dependencies.Interfaces,
                this.Dependencies.Packages,
                this.Conflicts,
                this.Hashes,
                this.Installation
                );
        }

        [JsonRequired]
        [JsonProperty("name")]
        public String PackageName { get; set; }

        [JsonProperty("authors")]
        public List<Author> Authors { get; set; } = new List<Author>();

        [JsonRequired]
        [JsonProperty("version")]
        public SemanticVersion Version { get; set; }

        [JsonProperty("givenVersion")]
        public String GivenVersion { get; set; } = "";

        [JsonRequired]
        [JsonProperty("arch")]
        public Arch @Arch { get; set; }

        [JsonRequired]
        [JsonProperty("side")]
        public CompatibilitySide Side { get; set; } = CompatibilitySide.Universal;

        [JsonRequired]
        [JsonProperty("interfaces")]
        public List<InterfaceProvision> Interfaces { get; set; } = new List<InterfaceProvision>();

        [JsonRequired]
        [JsonProperty("dependencies", ObjectCreationHandling = ObjectCreationHandling.Replace)]
        public BuildDependencySet Dependencies { get; set; }

        [JsonRequired]
        [JsonProperty("conflicts")]
        public List<Conflict> Conflicts { get; set; }

        [JsonProperty("hashes")]
        public List<Hash> Hashes { get; set; }

        /// <summary>
        /// Operations performed by this package in order to install. Can be reverted to uninstall.
        /// </summary>
        /// <value>
        /// The operations contained within. May be null on package repositories, but must exist in "package.json" files.
        /// </value>
        [JsonProperty("installation")]
        public List<ScriptFileDeclaration> Installation { get; set; }

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
                && Equals(this.Interfaces, other.Interfaces)
                && Equals(this.Dependencies, other.Dependencies)
                && Equals(this.Conflicts, other.Conflicts)
                && Equals(this.Hashes, other.Hashes)
                && Equals(this.Installation, other.Installation);
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
                hashCode = (hashCode * 397) ^ (this.Interfaces?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Dependencies.GetHashCode());
                hashCode = (hashCode * 397) ^ (this.Conflicts?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Hashes?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (this.Installation?.GetHashCode() ?? 0);
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
