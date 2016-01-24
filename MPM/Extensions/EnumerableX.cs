using System;
using System.Collections.Generic;
using System.Linq;

namespace MPM.Extensions {
    public static class EnumerableX {
        public static IEnumerable<T> Denull<T>(this IEnumerable<T> enumerable) => enumerable ?? Enumerable.Empty<T>();

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

        public static IReadOnlyCollection<T> Solidify<T>(this IEnumerable<T> enumerable) {
            return new SolidifyingReadOnlyCollection<T>(enumerable);
        }
    }
}
