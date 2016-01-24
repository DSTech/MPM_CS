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
        public readonly LiteDatabase Db;

        public LiteDbCacheManager(LiteDatabase db) {
            if ((this.Db = db) == null) {
                throw new ArgumentNullException(nameof(db));
            }
        }

        public IEnumerable<ICacheEntry> Entries {
            get { return Db.FileStorage.FindAll().Select(fileInfo => new LiteDbCacheEntry(this, fileInfo.Id)).ToArray(); }
        }

        public void Clear() {
            var fileStorage = Db.FileStorage;
            foreach (var entryId in fileStorage.FindAll().Select(fileInfo => fileInfo.Id).ToArray()) {
                fileStorage.Delete(entryId);
            }
        }

        public bool Contains(string cacheEntryName) {
            return Db.FileStorage.FindById(cacheEntryName) != null;
        }

        public void Delete(string cacheEntryName) {
            Db.FileStorage.Delete(cacheEntryName);
        }

        public ICacheEntry Fetch(string cacheEntryName) {
            var fileEntry = Db.FileStorage.FindById(cacheEntryName);
            if (fileEntry == null) {
                return null;
            }
            return new LiteDbCacheEntry(this, fileEntry.Id);
        }

        public void Store(string cacheEntryName, byte[] entryData) {
            using (var uploadStream = new MemoryStream(entryData, false)) {
                Db.FileStorage.Upload(new LiteFileInfo(cacheEntryName, cacheEntryName), uploadStream);
            }
        }

        private Stream GetStreamForFileId(string id) {
            var stream = Db.FileStorage.OpenRead(id);
            if (stream == null) {
                return null;
            }
            return stream;
        }

        private class LiteDbCacheEntry : ICacheEntry {
            public LiteDbCacheEntry(LiteDbCacheManager cacheManager, string id) {
                this.CacheManager = cacheManager;
                this.Id = id;
            }

            public LiteDbCacheManager CacheManager { get; set; }
            public string Id { get; set; }

            public Stream FetchStream() {
                var memstr = new MemoryStream();
                CacheManager.GetStreamForFileId(id: Id).CopyToAndClose(memstr);
                memstr.Seek(0, SeekOrigin.Begin);
                return memstr;
            }
        }
    }
}
