using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Data {
	public static class IUntypedKeyValueStoreExtensions {
		public static ITypedKeyValueStore<KEYTYPE, VALUETYPE> Typify<KEYTYPE, VALUETYPE>(this IUntypedKeyValueStore<KEYTYPE> keyValueStore) {
			return new TypifiedKeyValueStore<KEYTYPE, VALUETYPE>(keyValueStore);
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
