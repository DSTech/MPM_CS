using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using MPM.Data;
using MPM.Extensions;

namespace MPM.Core.Instances.Cache {
    //FUTURE: Replace standard filesystem usage with Platform.VirtualFileSystem
    public class FileSystemCacheManager : ICacheManager {
        public FileSystemCacheManager(string cachePath)
            : this(new DirectoryInfo(cachePath)) {
        }

        public FileSystemCacheManager(DirectoryInfo cacheDirectory) {
            this.cacheDirectory = cacheDirectory;
            if (!cacheDirectory.Exists) {
                cacheDirectory.Create();
            }
        }

        private IEnumerable<FileInfo> cacheEntryPaths => cacheDirectory.GetFiles();

        private DirectoryInfo cacheDirectory { get; }

        public IEnumerable<ICacheEntry> Entries {
            get {
                return cacheEntryPaths
                    .Select(entryPath => new CacheEntry(entryPath))
                    .ToArray();
            }
        }

        public void Clear() {
            foreach (var cachePathEntry in cacheEntryPaths) {
                File.Delete(cachePathEntry.FullName);
            }
        }

        public bool Contains(string cacheEntryName) => File.Exists(Path.Combine(cacheDirectory.FullName, cacheEntryName));

        public void Delete(string cacheEntryName) {
            ValidateEntryPath(cacheEntryName);
            File.Delete(Path.Combine(cacheDirectory.FullName, cacheEntryName));
        }

        public ICacheEntry Fetch(string cacheEntryName) {
            ValidateEntryPath(cacheEntryName);
            if (!Contains(cacheEntryName)) {
                return null;
            }
            return new CacheEntry(cacheDirectory.SubFile(cacheEntryName));
        }

        private static ICacheNamingConventionProvider _namingProvider = new StandardCacheNamingProvider();

        public ICacheNamingConventionProvider NamingProvider => _namingProvider;

        public void Store(string cacheEntryName, byte[] entryData) {
            ValidateEntryPath(cacheEntryName);
            var item = cacheDirectory.SubFile(cacheEntryName);
            Debug.Assert(item.Directory != null, "item.Directory != null");
            if (!item.Directory.Exists) {
                item.Directory.Create();
            }
            Directory.CreateDirectory(item.DirectoryName);
            File.WriteAllBytes(item.FullName, entryData);
        }

        private static void ValidateEntryPath([NotNull] string cacheEntryName) {
            if (cacheEntryName.Contains("..") || cacheEntryName.Contains(":/") || cacheEntryName.StartsWith("/")) {
                throw new InvalidOperationException("No parent directory access allowed.");
            }
        }

        private class CacheEntry : ICacheEntry {
            private FileInfo cacheFile;

            public CacheEntry(FileInfo cacheFile) {
                this.cacheFile = cacheFile;
            }

            public Stream FetchStream() {
                return File.OpenRead(cacheFile.FullName);
            }
        }
    }
}
