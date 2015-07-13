using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Newtonsoft.Json;

namespace MPM.Data {
	public class DbKeyValueStore<KEYTYPE> : BaseUntypedKeyValueStore<KEYTYPE>, IDisposable {
		private readonly IDbConnection db;
		private readonly string tableName;
		private JsonSerializerSettings serializerSettings = new JsonSerializerSettings() {
			TypeNameHandling = TypeNameHandling.Auto,
		};

		public DbKeyValueStore(IDbConnection db, String tableName) {
			if ((this.db = db) == null) {
				throw new ArgumentNullException(nameof(db));
			}
			if ((this.tableName = tableName) == null) {
				throw new ArgumentNullException(nameof(tableName));
			};
			this.tableName = this.tableName.Replace('`', '_');//No backticks. Because why are you doing that?
			InitializeTable();
		}

		private void InitializeTable() {
			try {
				db.Open();
				db.Execute($"CREATE TABLE IF NOT EXISTS `{tableName}` (`k` VARCHAR(255) NOT NULL primary key, `v` TEXT NOT NULL)");
			} finally {
				db.Close();
			}
		}
		private class KVEntry {
			public KVEntry() {
			}
			public KVEntry(String k, String v) {
				this.k = k;
				this.v = v;
			}
			public String k;
			public String v;
		}
		public override IEnumerable<KEYTYPE> Keys {
			get {
				try {
					db.Open();
					return db.Query<string>($"SELECT `k` FROM `{tableName}`")
						.Select(v => JsonConvert.DeserializeObject<KEYTYPE>(v, serializerSettings))
						.ToArray();
				} finally {
					db.Close();
				}
			}
		}
		public override IEnumerable<KeyValuePair<KEYTYPE, object>> Pairs {
			get {
				try {
					db.Open();
					return db.Query<KVEntry>($"SELECT `k`, `v` FROM `{tableName}`")
						.Select(entry =>
							new KeyValuePair<KEYTYPE, object>(
								JsonConvert.DeserializeObject<KEYTYPE>(entry.k, serializerSettings),
								JsonConvert.DeserializeObject(entry.v, serializerSettings)
							)
						)
						.ToArray();
				} finally {
					db.Close();
				}
			}
		}
		public override IEnumerable<object> Values {
			get {
				try {
					db.Open();
					return db.Query<string>($"SELECT `v` FROM `{tableName}`")
						.Select(v => JsonConvert.DeserializeObject(v, serializerSettings))
						.ToArray();
				} finally {
					db.Close();
				}
			}
		}

		public override void Clear(KEYTYPE key) {
			var keyStr = JsonConvert.SerializeObject(key, typeof(KEYTYPE), Formatting.None, serializerSettings);
			try {
				db.Open();
				db.Execute($"DELETE FROM {tableName} WHERE `k` = ?", new { keyStr = keyStr });
			} finally {
				db.Close();
			}
		}
		public override void Clear() {
			try {
				db.Open();
				db.Execute($"DELETE FROM {tableName}");
			} finally {
				db.Close();
			}
		}

		public override void Dispose() {
			Dispose(true);
		}

		bool disposed;
		protected virtual void Dispose(bool disposing) {
			if (disposed) {
				return;
			}
			if (disposing) {
				if (db != null) {
					db.Dispose();
				}
			}
			disposed = true;
		}

		public override object Get(KEYTYPE key, Type type) {
			var keyStr = JsonConvert.SerializeObject(key, typeof(KEYTYPE), Formatting.None, serializerSettings);
			try {
				db.Open();
				return db.Query<string>($"SELECT `v` FROM `{tableName}` WHERE `k` = ?", new { keyStr = keyStr })
					.Select(str => JsonConvert.DeserializeObject(str, type, serializerSettings))
					.FirstOrDefault();
			} finally {
				db.Close();
			}
		}

		public override void Set(KEYTYPE key, object value, Type type) {
			var keyStr = JsonConvert.SerializeObject(key, typeof(KEYTYPE), Formatting.None, serializerSettings);
			try {
				db.Open();
				if (value == null) {
					db.Execute($"DELETE FROM {tableName} WHERE `k` = ?", new { keyStr = keyStr });
					return;
				} else {
					var valStr = JsonConvert.SerializeObject(value, type, Formatting.None, serializerSettings);
					db.Execute($"INSERT OR REPLACE INTO `{tableName}` (`k`, `v`) VALUES (?, ?)", new { keyStr = keyStr, valStr = valStr });
				}
			} finally {
				db.Close();
			}
		}
	}
}
