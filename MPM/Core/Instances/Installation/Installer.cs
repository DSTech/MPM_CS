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
using MPM.Extensions;
using MPM.Types;
using MPM.Util;
using Newtonsoft.Json;
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
        ///     Installs a configuration afresh, ignoring pre-existing files. Updates the <paramref name="configuration"/> field.
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public void Install(InstanceConfiguration configuration) {
            configuration = configuration.SerialClone();

            //Download all packages if not cached
            foreach (var package in configuration.Packages) {
                var cacheEntryName = cacheManager.NamingProvider.GetNameForPackageArchive(package);
                if (cacheManager.Contains(cacheEntryName)) {
                    Console.WriteLine($"Package {package.PackageName} already cached.");
                    continue;
                }
                Console.WriteLine($"Downloading package {package.PackageName} to cache...");
                if (package.Hashes == null || package.Hashes.Count == 0) {
                    break;
                }
                var packageArchive = hashRepository.RetrieveArchive(package.PackageName, package.Hashes).WaitAndUnwrapException();
                cacheManager.Store(cacheEntryName, packageArchive);
            }

            var packageExtractor = new MPM.Archival.PackageExtractor(cacheManager);
            foreach (var package in configuration.Packages) {
                if (package.Installation != null && package.Installation.Count > 0) {
                    continue;
                }

                //Cache unarchived packages if they are not already extracted
                var entry = packageExtractor.ExtractToCacheIfNotExists(package);

                //Load installation scripts from embedded package.json files in downloaded packages
                using (var zip = new StreamingZipFetcher(entry.FetchStream())) {
                    package.Installation = JsonConvert.DeserializeObject<Build>(Encoding.UTF8.GetString(zip.FetchFile("package.json"))).Installation;
                }
            }

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
                Console.WriteLine($"\t{entry}");
                entry.Perform(fileSystem, entrySet.Key, cacheManager);
            }

            instance.Configuration = configuration;
        }
    }
}
