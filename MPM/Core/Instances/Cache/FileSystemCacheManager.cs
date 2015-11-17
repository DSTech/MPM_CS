using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MPM.Core.Instances.Cache {

	//TODO: Replace standard filesystem usage with Platform.VirtualFileSystem
	public class FileSystemCacheManager : ICacheManager {

		public FileSystemCacheManager(string cachePath) {
			this.cachePath = cachePath;
			Directory.CreateDirectory(cachePath);
		}

		public IEnumerable<ICacheEntry> Entries {
			get {
				return cacheEntryPaths
					.Select(entryPath => new CacheEntry(entryPath))
					.ToArray();
			}
		}

		private IEnumerable<string> cacheEntryPaths => Directory.GetFiles(cachePath);
		private string cachePath { get; }

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
			return new CacheEntry(Path.Combine(cachePath, cacheEntryName));
		}

		public void Store(string cacheEntryName, byte[] entryData) {
			ValidateEntryPath(cacheEntryName);
			var itemPath = Path.Combine(cachePath, cacheEntryName);
			Directory.CreateDirectory(Path.GetDirectoryName(itemPath));
			File.WriteAllBytes(itemPath, entryData);
		}

		private void ValidateEntryPath(string cacheEntryName) {
			if (cacheEntryName.Contains("..") || cacheEntryName.Contains(":/") || cacheEntryName.StartsWith("/")) {
				throw new InvalidOperationException("No parent directory access allowed.");
			}
		}

		private class CacheEntry : ICacheEntry {
			private string cachePath;

			public CacheEntry(string cachePath) {
				this.cachePath = cachePath;
			}

			public Stream FetchStream() {
				return File.OpenRead(cachePath);
			}
		}
	}
}
