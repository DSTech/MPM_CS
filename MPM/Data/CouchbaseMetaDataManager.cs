using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Couchbase.Lite;

namespace MPM.Data {
	public class CouchbaseMetaDataManager : IMetaDataManager, IDisposable {
		private Database db;

		public IEnumerable<string> Keys {
			get {
				using (var query = db.CreateAllDocumentsQuery()) {
					query.Prefetch = false;
					using (var queryEnumerator = query.Run()) {
						return queryEnumerator
							.Select(row => row.DocumentId)
							.ToArray();
					}
				}
			}
		}

		public CouchbaseMetaDataManager(Database db) {
			if ((this.db = db) == null) {
				throw new ArgumentNullException(nameof(db));
			}
		}
		public void Set(String key, object value, Type type) {
			if (value == null) {
				Clear(key);
				return;
			}
			var doc = db.GetDocument(key);
			doc.PutProperties(new Dictionary<string, object> {
				["value"] = Newtonsoft.Json.JsonConvert.SerializeObject(value, type, new Newtonsoft.Json.JsonSerializerSettings {
					TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
				}),
			});
		}
		public object Get(String key, Type type) {
			var doc = db.GetExistingDocument(key);
			if (doc == null) {
				return null;
			}
			var value = doc.GetProperty<string>("value");
			var deserialized = Newtonsoft.Json.JsonConvert.DeserializeObject(value, type, new Newtonsoft.Json.JsonSerializerSettings {
				TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto,
			});
			return deserialized;
		}

		public void Clear(String key) {
			db.DeleteLocalDocument(key);
		}

		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (db != null) {
					db.Dispose();
					db = null;
				}
			}
		}
	}
}
