using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MPM.Data;
using LiteDB;

namespace MPM.Data {
	public class LiteDbMetaDataManager : IMetaDataManager, IDisposable {
		private readonly LiteDB.LiteDatabase Db;
		private readonly string MetaCollectionName;
		private LiteCollection<MetaDataEntry> MetaCollection => Db.GetCollection<MetaDataEntry>(MetaCollectionName);

		public IEnumerable<string> Keys => MetaCollection.FindAll().Select(x => x.Key);

		public IEnumerable<KeyValuePair<string, object>> Pairs => MetaCollection
			.FindAll()
			.Select(x => new KeyValuePair<string, object>(x.Key, JsonConvert.DeserializeObject(x.Value)));

		public IEnumerable<object> Values => MetaCollection
			.FindAll()
			.Select(x => JsonConvert.DeserializeObject(x.Value));

		public LiteDbMetaDataManager(LiteDB.LiteDatabase db, string metaCollectionName) {
			if ((this.Db = db) == null) {
				throw new ArgumentNullException(nameof(db));
			}
			if ((this.MetaCollectionName = metaCollectionName) == null) {
				throw new ArgumentNullException(nameof(metaCollectionName));
			}
		}

		public void Set(String key, object value, Type type) {
			MetaCollection.Upsert(new KeyValuePair<string, string>(key, JsonConvert.SerializeObject(value, type, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })));
		}

		public void Set(string key, string value) {
			MetaCollection.Upsert(new KeyValuePair<string, string>(key, JsonConvert.SerializeObject(value, typeof(string), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })));
		}

		public string Get(string key) => MetaCollection.FindOne(kvp => kvp.Key == key)?.Value;

		public string Get(string key, string defaultValue) => MetaCollection.FindOne(kvp => kvp.Key == key)?.Value ?? defaultValue;

		public object Get(String key, Type type) {
			var value = Get(key);
			if (value != null) {
				return JsonConvert.DeserializeObject(value, type);
			}
			return value;
		}

		public void Clear(String key) => MetaCollection.Delete(kvp => kvp.Key == key);

		public void Clear() => MetaCollection.Delete(Query.All());

		public void Dispose() => Dispose(true);

		private bool disposed = false;
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (!disposed) {
					Db.Dispose();
					disposed = true;
				}
			}
		}

		public void Set<VALUETYPE>(string key, VALUETYPE value) {
			MetaCollection.Upsert(new MetaDataEntry(key, JsonConvert.SerializeObject(value, typeof(VALUETYPE), new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })));
		}

		public VALUETYPE Get<VALUETYPE>(string key) => (VALUETYPE)Get(key, typeof(VALUETYPE));

		public VALUETYPE Get<VALUETYPE>(string key, VALUETYPE defaultValue) {
			var value = Get<VALUETYPE>(key);
			if (value.Equals(default(VALUETYPE))) {
				return defaultValue;
			}
			return value;
		}
	}
}
