using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MPM {

	public class SolidifyingReadOnlyCollection<T> : IReadOnlyCollection<T> {
		private IEnumerable<T> source;
		private IReadOnlyCollection<T> solidified;
		public int Count => Cache().Count;

		public SolidifyingReadOnlyCollection(IEnumerable<T> source) {
			if (source == null) {
				throw new ArgumentNullException(nameof(source));
			}
			this.source = source;
			this.solidified = null;
			//Precache sources that do not need copied with ToArray
			var readOnlySource = source as IReadOnlyCollection<T>;
			if (readOnlySource != null) {
				solidified = readOnlySource;
			}
			var listSource = source as List<T>;
			if (listSource != null) {
				solidified = listSource;
			}
		}

		private IReadOnlyCollection<T> Cache() {
			if (solidified == null) {
				solidified = source.ToArray();
			}
			return solidified;
		}

		public IEnumerator<T> GetEnumerator() {
			return Cache().GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return Cache().GetEnumerator();
		}
	}
}
