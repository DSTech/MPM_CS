using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MPM.CLI;

namespace MPM.Extensions {

	class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> {
		private readonly IDictionary<TKey, TValue> dictionary;

		public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) {
			this.dictionary = dictionary;
		}

		public TValue this[TKey key] => dictionary[key];

		public int Count => dictionary.Count;

		public IEnumerable<TKey> Keys => dictionary.Keys;

		public IEnumerable<TValue> Values => dictionary.Values;

		public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => dictionary.GetEnumerator();

		public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);

		IEnumerator IEnumerable.GetEnumerator() => dictionary.GetEnumerator();
	}

	public static class IDictionaryExtensions {
		public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
			return new ReadOnlyDictionary<TKey, TValue>(dictionary);
		}
	}
}
