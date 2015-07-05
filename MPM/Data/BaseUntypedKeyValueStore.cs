using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Data {
	public abstract class BaseUntypedKeyValueStore<KEYTYPE> : IUntypedKeyValueStore<KEYTYPE> {
		public abstract IEnumerable<KEYTYPE> Keys { get; }
		public abstract IEnumerable<KeyValuePair<KEYTYPE, object>> Pairs { get; }
		public abstract IEnumerable<object> Values { get; }
		public abstract object Get(KEYTYPE key, Type type);
		public abstract void Set(KEYTYPE key, object value, Type type);
		public abstract void Clear(KEYTYPE key);

		public abstract void Dispose();

		public VALUETYPE Get<VALUETYPE>(KEYTYPE key) {
			return (VALUETYPE)Get(key, typeof(VALUETYPE));
		}
		public VALUETYPE Get<VALUETYPE>(KEYTYPE key, VALUETYPE defaultValue) {
			var fetched = Get<VALUETYPE>(key);
			if (fetched.Equals(default(VALUETYPE))) {
				return defaultValue;
			}
			return fetched;
		}
		public void Set<VALUETYPE>(KEYTYPE key, VALUETYPE value) {
			Set(key, value, typeof(VALUETYPE));
		}
	}
}
