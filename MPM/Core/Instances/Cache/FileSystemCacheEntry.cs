using System;
using System.IO;

namespace MPM.Core.Instances.Cache {

	public class FileSystemCacheEntry : ICacheEntry {
		private string cachePath;

		public FileSystemCacheEntry(string cachePath) {
			this.cachePath = cachePath;
		}

		public Stream FetchStream() {
			return File.OpenRead(cachePath);
		}
	}
}
