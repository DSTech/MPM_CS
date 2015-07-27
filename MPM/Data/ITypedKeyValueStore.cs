using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Data {

	public interface ITypedKeyValueStore<KEYTYPE, VALUETYPE> : IDisposable {
		IEnumerable<KEYTYPE> Keys { get; }
		IEnumerable<KeyValuePair<KEYTYPE, VALUETYPE>> Pairs { get; }
		IEnumerable<VALUETYPE> Values { get; }

		void Set(KEYTYPE key, VALUETYPE value);

		VALUETYPE Get(KEYTYPE key);

		VALUETYPE Get(KEYTYPE key, VALUETYPE defaultValue);

		void Clear(KEYTYPE key);

		void Clear();
	}
}
