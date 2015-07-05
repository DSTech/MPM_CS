using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Data {
	public interface IUntypedKeyValueStore<KEYTYPE> : IDisposable {
		IEnumerable<KEYTYPE> Keys { get; }
		IEnumerable<KeyValuePair<KEYTYPE, object>> Pairs { get; }
		IEnumerable<object> Values { get; }
		void Set(KEYTYPE key, object value, Type type);
		void Set<VALUETYPE>(KEYTYPE key, VALUETYPE value);
		object Get(KEYTYPE key, Type type);
		VALUETYPE Get<VALUETYPE>(KEYTYPE key);
		VALUETYPE Get<VALUETYPE>(KEYTYPE key, VALUETYPE defaultValue);
		void Clear(KEYTYPE key);
	}
}
