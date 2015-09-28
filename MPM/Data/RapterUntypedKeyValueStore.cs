using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RaptorDB;

namespace MPM.Data {
	public class RaptorUntypedKeyValueStore<T> : IUntypedKeyValueStore<T> where T : IComparable<T> {
		public readonly KeyStore<T> raptor;
		public RaptorUntypedKeyValueStore(KeyStore<T> raptor) {
			this.raptor = raptor;
		}
		public IEnumerable<T> Keys => raptor.Enumerate(default(T)).Select(x => x.Key);

		public IEnumerable<KeyValuePair<T, object>> Pairs =>
			raptor
				.Enumerate(default(T))
				.Select(x => new KeyValuePair<T, object>(x.Key, Get<object>(x.Key)));

		public IEnumerable<object> Values =>
			raptor
				.Enumerate(default(T))
				.Select(x => Get<object>(x.Key));

		public void Clear() {
			foreach (var key in Keys) {
				raptor.RemoveKey(key);
			}
		}

		public void Clear(T key) {
			raptor.RemoveKey(key);
		}

		public void Dispose() {
			raptor.Dispose();
		}

		public object Get(T key, Type type) {
			byte[] val;
			if (!raptor.Get(key, out val)) {
				return null;
			}
			var strVal = Encoding.UTF8.GetString(val);
			var objVal = JsonConvert.DeserializeObject(strVal, type, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
			return objVal;
		}

		public VALUETYPE Get<VALUETYPE>(T key) {
			byte[] val;
			if (!raptor.Get(key, out val)) {
				return default(VALUETYPE);
			}
			var strVal = Encoding.UTF8.GetString(val);
			var objVal = JsonConvert.DeserializeObject<VALUETYPE>(strVal, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
			return objVal;
		}

		public VALUETYPE Get<VALUETYPE>(T key, VALUETYPE defaultValue) {
			byte[] val;
			if (!raptor.Get(key, out val)) {
				return defaultValue;
			}
			var strVal = Encoding.UTF8.GetString(val);
			var objVal = JsonConvert.DeserializeObject<VALUETYPE>(strVal, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
			return objVal;
		}

		public void Set(T key, object value, Type type) {
			raptor.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, type, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })));
		}

		public void Set<VALUETYPE>(T key, VALUETYPE value) {
			raptor.Set(key, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(value, typeof(VALUETYPE), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })));
		}
	}
}
