using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MPM.Types {
    public struct BuildDependencySet : IEquatable<BuildDependencySet> {
        #region Equality members

        public bool Equals(BuildDependencySet other) {
            return Equals(this.Interfaces, other.Interfaces) && Equals(this.Packages, other.Packages);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            return obj is BuildDependencySet && this.Equals((BuildDependencySet) obj);
        }

        public override int GetHashCode() {
            unchecked { return ((this.Interfaces != null ? this.Interfaces.GetHashCode() : 0) * 397) ^ (this.Packages != null ? this.Packages.GetHashCode() : 0); }
        }

        public static bool operator ==(BuildDependencySet left, BuildDependencySet right) {
            return left.Equals(right);
        }

        public static bool operator !=(BuildDependencySet left, BuildDependencySet right) {
            return !left.Equals(right);
        }

        #endregion

        [JsonProperty("interfaces", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private List<InterfaceDependency> _interfaces;

        [JsonIgnore]
        public List<InterfaceDependency> Interfaces {
            get {
                return _interfaces ?? new List<InterfaceDependency>();
            }
            set {
                if (value == null || value.Count == 0) {
                    _interfaces = null;
                }
                _interfaces = value;
            }
        }

        [JsonProperty("packages", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private List<PackageDependency> _packages;

        [JsonIgnore]
        public List<PackageDependency> Packages {
            get {
                return _packages ?? new List<PackageDependency>();
            }
            set {
                if (value == null || value.Count == 0) {
                    _packages = null;
                }
                _packages = value;
            }
        }

    }
}