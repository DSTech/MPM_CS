namespace MPM.Extensions {
    using System.Collections;
    using System.Collections.Generic;
    internal class ReadOnlyDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue> {
        private readonly IDictionary<TKey, TValue> dictionary;

        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary) {
            this.dictionary = new Dictionary<TKey, TValue>(dictionary);
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
}

namespace System.Collections.Generic {

    public static class IDictionaryExtensions {
        public static IReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dictionary) {
            return new MPM.Extensions.ReadOnlyDictionary<TKey, TValue>(dictionary);
        }
    }
}
