using System.Collections.Generic;
using System.Linq;
using MPM.Archival;
using MPM.Core.Instances.Cache;
using MPM.Extensions;

namespace MPM.Core.Instances.Installation {
    public abstract class ArchInstallationOperation {
        protected ArchInstallationOperation(string packageName, SemVer.Version packageVersion, ICacheManager cacheManager) {
            this.PackageName = packageName;
            this.Cache = cacheManager;
        }

        public string PackageName { get; set; }
        public SemVer.Version PackageVersion { get; set; }
        public ICacheManager Cache { get; set; }

        public abstract IFileMap GenerateOperations();
    }

    /// <summary>
    ///     Extracts an entire cached archive, excluding any ignored paths, to a location
    /// </summary>
    public class ExtractArchInstallationOperation : ArchInstallationOperation {
        public ExtractArchInstallationOperation(
            string packageName,
            SemVer.Version packageVersion,
            ICacheManager cacheManager,
            string cachedName,
            string sourcePath,
            string targetPath,
            IEnumerable<string> ignorePaths = null
            ) : base(packageName, packageVersion, cacheManager) {
            this.IgnorePaths = ignorePaths.Denull();
            this.SourcePath = sourcePath;
            this.CacheEntryName = cachedName;
            this.TargetPath = targetPath;
        }

        //The targetted file or the entire targetted directory will be extracted where not starting with an ignored path.
        //Use "" to copy the entire archive.
        public string SourcePath { get; set; }

        //Directories will be merged, files will be overwritten.
        public string TargetPath { get; set; }

        public string CacheEntryName { get; set; }

        public IEnumerable<string> IgnorePaths { get; set; }

        public override IFileMap GenerateOperations() {
            //Iterate each path in the archive, building a filemap of extract operations to perform.
            var fileMap = new DictionaryFileMap();
            using (var zip = new SeekingZipFetcher(Cache.Fetch(CacheEntryName).FetchStream())) {
                foreach (var entry in zip.FetchEntryInfo()) {
                    if (entry.IsDirectory) {
                        continue;
                    }
                    var entryName = entry.Name;
                    if (!entryName.StartsWith(SourcePath) || IgnorePaths.Any(ignorePath => entryName.StartsWith(ignorePath))) {
                        continue;
                    }
                    var targetPath = TargetPath + entry.Name.Substring(SourcePath.Length);
                    var exOp = new ExtractFileOperation(PackageName, PackageVersion, CacheEntryName, entryName);
                    fileMap.Register(targetPath, exOp);
                }
            }
            return fileMap;
        }
    }

    /// <summary>
    ///     Extracts a single file from the specified cached archive to a location
    /// </summary>
    public class ExtractSingleArchInstallationOperation : ArchInstallationOperation {
        public ExtractSingleArchInstallationOperation(
            string packageName,
            SemVer.Version packageVersion,
            ICacheManager cacheManager,
            string cachedName,
            string sourcePath,
            string targetPath
            ) : base(packageName, packageVersion, cacheManager) {
            this.CachedName = cachedName;
            this.SourcePath = sourcePath;
            this.TargetPath = targetPath;
        }

        public string CachedName { get; set; }
        public string SourcePath { get; set; }
        public string TargetPath { get; set; }

        public override IFileMap GenerateOperations() {
            //Build an extract operation for the given archive that extracts the source.
            var exOp = new ExtractFileOperation(PackageName, PackageVersion, CachedName, SourcePath);
            return new SingleEntryFileMap(TargetPath, exOp);
        }
    }

    public class CopyArchInstallationOperation : ArchInstallationOperation {
        public CopyArchInstallationOperation(
            string packageName,
            SemVer.Version packageVersion,
            ICacheManager cacheManager,
            string cachedName,
            string targetPath
            ) : base(packageName, packageVersion, cacheManager) {
            this.CachedName = cachedName;
            this.TargetPath = targetPath;
        }

        public string CachedName { get; set; }
        public string TargetPath { get; set; }

        public override IFileMap GenerateOperations() {
            //Build a copy operation for the given file that copies the source to the destination.
            var copyOp = new CopyFileOperation(PackageName, PackageVersion, CachedName);
            return new SingleEntryFileMap(TargetPath, copyOp);
        }
    }
}
