using System;
using System.Collections.Generic;
using System.IO;

namespace MPM.Core.Instances.Cache {
	public class FileSystemCacheManager : ICacheManager {
		private string cachePath { get; }

		public FileSystemCacheManager(string cachePath) {
			this.cachePath = cachePath;
		}

		public IEnumerable<ICacheEntry> Entries {
			get {
				throw new NotImplementedException();
			}
		}

		public void Clear() {
			throw new NotImplementedException();
		}

		public void Contains(string cacheEntryName) {
			throw new NotImplementedException();
		}

		public void Delete(string cacheEntryName) {
			throw new NotImplementedException();
		}

		public ICacheEntry Fetch(string cacheEntryName) {
			throw new NotImplementedException();
		}

		public void Store(string cacheEntryName, byte[] entryData) {
			throw new NotImplementedException();
		}
	}
}
