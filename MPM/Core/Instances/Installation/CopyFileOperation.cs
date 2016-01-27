using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using MPM.Archival;
using MPM.Core.Instances.Cache;
using Platform.VirtualFileSystem;

namespace MPM.Core.Instances.Installation {
    /// <summary>
    ///     An operation to extract a file from a cached archive to a specified target.
    /// </summary>
    public class CopyFileOperation : IFileOperation {
        public CopyFileOperation() {
        }

        public CopyFileOperation(string packageName, SemVer.Version packageVersion, string cacheEntryName) {
            this.PackageName = PackageName;
            this.PackageVersion = packageVersion;
            this.CacheEntryName = cacheEntryName;
        }

        public bool UsesPreviousContents => false;

        public string CacheEntryName { get; set; }

        public string PackageName { get; set; }

        public SemVer.Version PackageVersion { get; set; }

        public void Perform(IFileSystem fileSystem, String path, ICacheReader cache) {
            var targetFile = fileSystem.ResolveFile(path);
            if (targetFile.Exists) {
                targetFile.Delete();
            }
            var targetDir = targetFile.ResolveDirectory(".");
            if (!targetDir.Exists) {
                targetDir.Create(true);
            }
            var cacheEntry = cache.Fetch(CacheEntryName);
            if (cacheEntry == null) {
                throw new KeyNotFoundException($"Cache did not contain an entry for {CacheEntryName}");
            }
            using (var entryStream = cacheEntry.FetchStream()) {
                using (var fileWriter = targetFile.GetContent().GetOutputStream(System.IO.FileMode.Create)) {
                    entryStream.CopyTo(fileWriter);
                }
            }
        }

        public override string ToString() => $"<Copy {CacheEntryName}>";
    }
}
