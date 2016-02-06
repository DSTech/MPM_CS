using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Dependency;
using MPM.Core.Instances.Cache;
using MPM.Core.Instances.Info;
using MPM.Core.Protocols;
using MPM.Data;
using MPM.Data.Repository;
using MPM.Types;
using Nito.AsyncEx.Synchronous;
using Platform.VirtualFileSystem;

namespace MPM.Core.Instances.Installation {
    public class Installer {
        private readonly ICacheManager cacheManager;
        private readonly IFileSystem fileSystem;
        private readonly IHashRepository hashRepository;
        private readonly Instance instance;
        private readonly IPackageRepository packageRepository;
        private readonly IProtocolResolver protocolResolver;

        public Installer(Instance instance, IPackageRepository packageRepository, IHashRepository hashRepository, ICacheManager cacheManager, IProtocolResolver protocolResolver) {
            this.instance = instance;
            this.packageRepository = packageRepository;
            this.hashRepository = hashRepository;
            this.fileSystem = instance.FetchFileSystem();
            this.cacheManager = cacheManager;
            this.protocolResolver = protocolResolver;
        }

        /// <summary>
        ///     Installs a configuration afresh, ignoring pre-existing files.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public void Install(InstanceConfiguration configuration) {
            //Download all packages if not cached
            foreach (var package in configuration.Packages) {
                var cacheEntryName = cacheManager.NamingProvider.GetNameForPackageArchive(package);
                if (cacheManager.Contains(cacheEntryName)) {
                    Console.WriteLine($"Package {package.PackageName} already cached.");
                    continue;
                }
                Console.WriteLine($"Downloading package {package.PackageName} to cache...");
                if (package.Hashes == null || package.Hashes.Count == 0) {
                    throw new Exception("Package contained no hashes to download");
                }
                var packageArchive = hashRepository.RetrieveArchive(package.PackageName, package.Hashes).WaitAndUnwrapException();
                cacheManager.Store(cacheEntryName, packageArchive);
            }

            //Cache unarchived packages if they are not already extracted
            var packageExtractor = new MPM.Archival.PackageExtractor(cacheManager);
            foreach (var package in configuration.Packages) {
                if (package.Installation != null && package.Installation.Count > 0) {
                    continue;
                }
                packageExtractor.ExtractToCacheIfNotExists(package);
            }

            //Load installation scripts from embedded package.json files in downloaded packages
            //TODO: load installation scripts

            //Generate operation listings for installation
            var fileMap = configuration.GenerateFileMap(cacheManager, protocolResolver);
            foreach (var opTarget in fileMap) {
                var target = opTarget.Key;
                foreach (var operation in opTarget.Value) {
                    operation.Perform(fileSystem, target, cacheManager);
                }
            }

            //Perform operations in order
            foreach (var entrySet in fileMap) {
                var entry = entrySet.Value.LastOrDefault();
                if (entry == null) {
                    continue;
                }
                entry.Perform(fileSystem, entrySet.Key, cacheManager);
            }
        }
    }
}

namespace MPM.Archival {
}
