using System;
using System.Collections.Generic;

public static class Equality<T> {
    public static IEqualityComparer<T> CreateComparer<V>(Func<T, V> keySelector) =>
        CreateComparer(keySelector, null);

    public static IEqualityComparer<T> CreateComparer<V>(Func<T, V> keySelector, IEqualityComparer<V> comparer) =>
        new KeyEqualityComparer<V>(keySelector, comparer);

    private class KeyEqualityComparer<V> : IEqualityComparer<T> {
        private readonly IEqualityComparer<V> comparer;
        private readonly Func<T, V> keySelector;

        public KeyEqualityComparer(Func<T, V> keySelector, IEqualityComparer<V> comparer) {
            if (keySelector == null) {
                throw new ArgumentNullException(nameof(keySelector));
            }

            this.keySelector = keySelector;
            this.comparer = comparer ?? EqualityComparer<V>.Default;
        }

        public bool Equals(T x, T y) => comparer.Equals(keySelector(x), keySelector(y));

        public int GetHashCode(T obj) => comparer.GetHashCode(keySelector(obj));
    }
}
