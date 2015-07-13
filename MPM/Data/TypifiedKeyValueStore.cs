using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Data {
	public struct KeyValueStoreTypifier<KEYTYPE> {
		public IUntypedKeyValueStore<KEYTYPE> UntypedValueStore { get; }
		public KeyValueStoreTypifier(IUntypedKeyValueStore<KEYTYPE> untypedValueStore) {
			this.UntypedValueStore = untypedValueStore;
		}
		public TypifiedKeyValueStore<KEYTYPE, VALUETYPE> As<VALUETYPE>() {
			return new TypifiedKeyValueStore<KEYTYPE, VALUETYPE>(UntypedValueStore);
		}
	}
	public static class IUntypedKeyValueStoreExtensions {
		public static KeyValueStoreTypifier<KEYTYPE> Typify<KEYTYPE>(this IUntypedKeyValueStore<KEYTYPE> keyValueStore) {
			return new KeyValueStoreTypifier<KEYTYPE>(keyValueStore);
		}
	}
	public class TypifiedKeyValueStore<KEYTYPE, VALUETYPE> : ITypedKeyValueStore<KEYTYPE, VALUETYPE> {
		private IUntypedKeyValueStore<KEYTYPE> store { get; }

		public TypifiedKeyValueStore(IUntypedKeyValueStore<KEYTYPE> keyValueStore) {
			this.store = keyValueStore;
		}

		public IEnumerable<KEYTYPE> Keys {
			get {
				return store.Keys;
			}
		}

		public IEnumerable<KeyValuePair<KEYTYPE, VALUETYPE>> Pairs {
			get {
				return store.Pairs.Select(pair => new KeyValuePair<KEYTYPE, VALUETYPE>(pair.Key, (VALUETYPE)pair.Value));
			}
		}

		public IEnumerable<VALUETYPE> Values {
			get {
				return store.Values.Cast<VALUETYPE>();
			}
		}

		public void Clear(KEYTYPE key) {
			store.Clear(key);
		}
		public void Clear() {
			store.Clear();
		}

		public void Dispose() {
			store.Dispose();
		}

		public VALUETYPE Get(KEYTYPE key) {
			return store.Get<VALUETYPE>(key);
		}

		public VALUETYPE Get(KEYTYPE key, VALUETYPE defaultValue) {
			return store.Get(key, defaultValue);
		}

		public void Set(KEYTYPE key, VALUETYPE value) {
			store.Set(key, value);
		}
	}
}
