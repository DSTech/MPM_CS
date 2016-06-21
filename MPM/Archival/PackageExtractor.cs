using System;
using System.IO;
using System.Linq;
using MPM.Core.Instances.Cache;
using MPM.Types;

namespace MPM.Archival {
    public class PackageExtractor {
        private readonly ICacheManager cacheManager;
        public PackageExtractor(ICacheManager cacheManager) {
            this.cacheManager = cacheManager;
        }

        public ICacheEntry ExtractToCacheIfNotExists(Build package) {
            var unarchivedName = cacheManager.NamingProvider.GetNameForPackageUnarchived(package);
            var archivedName = cacheManager.NamingProvider.GetNameForPackageArchive(package);
            var extracted = cacheManager.Fetch(unarchivedName);
            if (extracted != null) {
                return extracted;
            }
            var unextractedStream = cacheManager.Fetch(archivedName).FetchStream();
            using (var unarchivedFile = new MemoryStream()) {
                Archive.Unpack(unextractedStream, package.PackageName, unarchivedFile);
                cacheManager.Store(unarchivedName, unarchivedFile.ToArrayFromStart());
            }
            return cacheManager.Fetch(unarchivedName);
        }
    }
}