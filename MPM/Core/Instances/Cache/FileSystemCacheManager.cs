using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MPM.Core.Instances.Cache {

	//TODO: Replace standard filesystem usage with Platform.VirtualFileSystem
	public class FileSystemCacheManager : ICacheManager {
		private string cachePath { get; }

		private IEnumerable<string> cacheEntryPaths => Directory.GetFiles(cachePath);

		private void ValidateEntryPath(string cacheEntryName) {
			if (cacheEntryName.Contains("..") || cacheEntryName.Contains(":/") || cacheEntryName.StartsWith("/")) {
				throw new InvalidOperationException("No parent directory access allowed.");
			}
		}

		public FileSystemCacheManager(string cachePath) {
			this.cachePath = cachePath;
		}

		public IEnumerable<ICacheEntry> Entries {
			get {
				return cacheEntryPaths
					.Select(entryPath => new FileSystemCacheEntry(entryPath))
					.ToArray();
			}
		}

		public void Clear() {
			foreach (var cachePathEntry in cacheEntryPaths) {
				File.Delete(cachePathEntry);
			}
		}

		public bool Contains(string cacheEntryName) => File.Exists(Path.Combine(cachePath, cacheEntryName));

		public void Delete(string cacheEntryName) {
			ValidateEntryPath(cacheEntryName);
			File.Delete(Path.Combine(cachePath, cacheEntryName));
		}

		public ICacheEntry Fetch(string cacheEntryName) {
			ValidateEntryPath(cacheEntryName);
			if (!Contains(cacheEntryName)) {
				return null;
			}
			return new FileSystemCacheEntry(Path.Combine(cachePath, cacheEntryName));
		}

		public void Store(string cacheEntryName, byte[] entryData) {
			ValidateEntryPath(cacheEntryName);
			File.WriteAllBytes(Path.Combine(cachePath, cacheEntryName), entryData);
		}
	}
}
