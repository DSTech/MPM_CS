using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using MPM.Core.Instances.Cache;
using System.Reactive.Disposables;
using MPM.Extensions;

namespace MPM.Data {
	public class LiteDbCacheManager : ICacheManager {
		private class LiteDbCacheEntry : ICacheEntry {
			//Change to use connection string
			private readonly string ConnectionString;
			public readonly string Id;
			public LiteDbCacheEntry(string connectionString, string id) {
				this.ConnectionString = connectionString;
				this.Id = id;
			}
			public Stream FetchStream() {
				var memstr = new MemoryStream();
				using (var db = new LiteDatabase(ConnectionString)) {
					db.FileStorage.FindById(id: Id).OpenRead().CopyTo(memstr);
					memstr.Seek(0, SeekOrigin.Begin);
				}
				return memstr;
			}
		}

		public readonly String ConnectionString;

		public LiteDbCacheManager(string connectionString) {
			this.ConnectionString = connectionString;
		}

		public IEnumerable<ICacheEntry> Entries {
			get {
				using (var db = new LiteDatabase(ConnectionString)) {
					return db.FileStorage.FindAll().Select(fileInfo => new LiteDbCacheEntry(ConnectionString, fileInfo.Id)).ToArray();
				}
			}
		}

		public void Clear() {
			using (var db = new LiteDatabase(ConnectionString)) {
				var fileStorage = db.FileStorage;
				foreach (var entryId in fileStorage.FindAll().Select(fileInfo => fileInfo.Id).ToArray()) {
					fileStorage.Delete(entryId);
				}
			}
		}

		public bool Contains(string cacheEntryName) {
			using (var db = new LiteDatabase(ConnectionString)) {
				return db.FileStorage.FindById(cacheEntryName) != null;
			}
		}

		public void Delete(string cacheEntryName) {
			using (var db = new LiteDatabase(ConnectionString)) {
				db.FileStorage.Delete(cacheEntryName);
			}
		}

		public ICacheEntry Fetch(string cacheEntryName) {
			using (var db = new LiteDatabase(ConnectionString)) {
				var fileEntry = db.FileStorage.FindById(cacheEntryName);
				if (fileEntry == null) {
					return null;
				}
				return new LiteDbCacheEntry(ConnectionString, fileEntry.Id);
			}
		}

		public void Store(string cacheEntryName, byte[] entryData) {
			using (var db = new LiteDatabase(ConnectionString)) {
				using (var uploadStream = new MemoryStream(entryData, false)) {
					db.FileStorage.Upload(new LiteFileInfo(cacheEntryName, cacheEntryName), uploadStream);
				}
			}
		}
	}
}
