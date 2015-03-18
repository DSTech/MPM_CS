using System;
using System.Collections.Generic;

public static class Equality<T> {
	public static IEqualityComparer<T> CreateComparer<V>(Func<T, V> keySelector) {
		return CreateComparer(keySelector, null);
	}

	public static IEqualityComparer<T> CreateComparer<V>(Func<T, V> keySelector, IEqualityComparer<V> comparer) {
		return new KeyEqualityComparer<V>(keySelector, comparer);
	}

	class KeyEqualityComparer<V> : IEqualityComparer<T> {
		readonly Func<T, V> keySelector;
		readonly IEqualityComparer<V> comparer;

		public KeyEqualityComparer(Func<T, V> keySelector, IEqualityComparer<V> comparer) {
			if (keySelector == null)
				throw new ArgumentNullException("keySelector");

			this.keySelector = keySelector;
			this.comparer = comparer ?? EqualityComparer<V>.Default;
		}

		public bool Equals(T x, T y) {
			return comparer.Equals(keySelector(x), keySelector(y));
		}

		public int GetHashCode(T obj) {
			return comparer.GetHashCode(keySelector(obj));
		}
	}
}
