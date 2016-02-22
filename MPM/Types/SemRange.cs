using System;
using SemVer;

namespace MPM.Types {
    //TODO: Convert to using SemVersion range equality, added via next release of SemanticVersioning
    [Newtonsoft.Json.JsonConverter(typeof(MPM.Util.Json.VersionRangeConverter))]
    public class SemRange : SemVer.Range, IEquatable<SemRange> {
        public SemRange(string rangeSpec, bool loose = false)
            : base(rangeSpec, loose) {
        }

        #region Equality members

        public bool Equals(SemRange other) => this.ToString() == other.ToString();

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != this.GetType()) { return false; }
            return Equals((SemRange) obj);
        }

        public override int GetHashCode() => this.ToString().GetHashCode();

        public static bool operator ==(SemRange left, SemRange right) {
            return Equals(left, right);
        }

        public static bool operator !=(SemRange left, SemRange right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
