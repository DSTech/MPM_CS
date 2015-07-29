using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Extensions {

	public static class EnumeratorExtensions {

		public static IEnumerable<T> TakeApply<T>(this IEnumerable<T> sequence, int count, Action<IEnumerable<T>> callback) {
			var enumr = sequence.GetEnumerator();
			callback.Invoke(enumr.Take(count));
			return enumr.AsEnumerable();//Takes care of disposal after enumeration ends
		}

		public static Y TakeSelect<T, Y>(this IEnumerable<T> sequence, int count, Func<IEnumerable<T>, IEnumerable<T>, Y> callback) {
			var enumr = sequence.GetEnumerator();
			return callback.Invoke(enumr.Take(count), enumr.AsEnumerable());
		}

		public static IEnumerable<T> TakeSave<T>(this IEnumerable<T> sequence, int count, out T[] destination) {
			var enumr = sequence.GetEnumerator();
			destination = enumr.Take(count).ToArray();
			return enumr.AsEnumerable();
		}

		public static IEnumerable<T> Take<T>(this IEnumerator<T> enumerator, int count) {
			for (int i = 0; i < count; ++i) {
				if (!enumerator.MoveNext()) {
					using (enumerator) {
						break;
					}
				}
				yield return enumerator.Current;
			}
		}

		public static IEnumerable<T> AsEnumerable<T>(this IEnumerator<T> enumerator) {
			using (enumerator) {
				while (enumerator.MoveNext()) {
					yield return enumerator.Current;
				}
			}
		}
	}
}
