using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace MPM.Extensions {
    public static class EnumerableX {
        [NotNull] public static IEnumerable<T> Denull<T>([CanBeNull] this IEnumerable<T> enumerable) => enumerable ?? Enumerable.Empty<T>();

        [NotNull] public static T[] DenullArray<T>([CanBeNull] this IEnumerable<T> enumerable) => enumerable?.ToArray() ?? new T[0];

        [NotNull] public static List<T> DenullList<T>([CanBeNull] this IEnumerable<T> enumerable) => enumerable?.ToList() ?? new List<T>();

        public static IEnumerable<T> SubEnumerable<T>(this IEnumerable<T> enumerable, int startIndex) {
            if (enumerable == null) {
                return null;
            }
            if (startIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
            if (startIndex == 0) {
                return enumerable;
            }
            return enumerable.Skip(startIndex);
        }

        public static IEnumerable<T> SubEnumerable<T>(this IEnumerable<T> enumerable, int startIndex, int count) {
            if (enumerable == null) {
                return null;
            }
            if (startIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }
            if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if (count == 0) {
                return Enumerable.Empty<T>();
            }
            if (startIndex == 0) {
                return enumerable.Take(count);
            }
            return enumerable.Skip(startIndex).Take(count);
        }

        public static IReadOnlyCollection<T> Solidify<T>(this IEnumerable<T> enumerable) => new Util.SolidifyingReadOnlyCollection<T>(enumerable);

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable) => new HashSet<T>(enumerable);

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> comparer) => new HashSet<T>(enumerable, comparer);
    }
}
