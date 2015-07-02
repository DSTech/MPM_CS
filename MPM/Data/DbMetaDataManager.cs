using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using System.Data.SQLite.Generic;
using System.Data.SQLite.Linq;

namespace MPM.Data {
	public class DbMetaDataManager : IMetaDataManager, IDisposable {
		private IUntypedKeyValueStore<String> db;

		public IEnumerable<string> Keys {
			get {
				return db.Keys;
			}
		}
		public IEnumerable<KeyValuePair<string, object>> Pairs {
			get {
				return db.Pairs;
			}
		}

		public DbMetaDataManager(IDbConnection db, string tableName) {
			if (tableName == null) {
				throw new ArgumentNullException(nameof(tableName));
			}
			if ((this.db = new DbKeyValueStore<String>(db, tableName)) == null) {
				throw new ArgumentNullException(nameof(db));
			}
		}
		public DbMetaDataManager(IUntypedKeyValueStore<String> db) {
			if ((this.db = db) == null) {
				throw new ArgumentNullException(nameof(db));
			}
		}
		public void Set(String key, object value, Type type) {
			db.Set(key, value, type);
		}

		public void Set(string key, string value) {
			db.Set(key, value);
		}

		public string Get(string key) {
			return db.Get<string>(key);
		}

		public string Get(string key, string defaultValue) {
			return db.Get(key, defaultValue);
		}

		public object Get(String key, Type type) {
			return db.Get(key, type);
		}

		public void Clear(String key) {
			db.Clear(key);
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

		public void Set<VALUETYPE>(string key, VALUETYPE value) {
			db.Set(key, value);
		}

		public VALUETYPE Get<VALUETYPE>(string key) {
			return db.Get<VALUETYPE>(key);
		}

		public VALUETYPE Get<VALUETYPE>(string key, VALUETYPE defaultValue) {
			return db.Get<VALUETYPE>(key, defaultValue);
		}
	}
}
