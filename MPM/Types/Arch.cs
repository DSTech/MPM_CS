using System;

namespace MPM.Types {

	public class Arch : IEquatable<Arch> {

		public Arch(string archId) {
			this.Id = archId;
		}

		public string Id { get; }

		public bool Equals(Arch other) {
			if (other == null) return false;
			return (this.Id.Equals(other.Id));
		}

		public override bool Equals(object obj) => Equals(obj as Arch);

		public override int GetHashCode() {
			return new Tuple<string>(Id).GetHashCode();
		}

		public static bool operator ==(Arch first, Arch second) {
			if (object.ReferenceEquals(first, second)) return true;
			if (object.ReferenceEquals(first, null)) return false;
			if (object.ReferenceEquals(second, null)) return false;

			return first.Equals(second);
		}

		public static bool operator !=(Arch first, Arch second) {
			if (object.ReferenceEquals(first, second)) return false;
			if (object.ReferenceEquals(first, null)) return true;
			if (object.ReferenceEquals(second, null)) return true;

			return !first.Equals(second);
		}
	}
}
