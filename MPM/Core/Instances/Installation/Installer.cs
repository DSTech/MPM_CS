using System;
using System.Linq;
using System.Text;
using MPM.Core.Dependency;
using MPM.Core.Instances.Cache;
using MPM.Core.Protocols;
using MPM.Data.Repository;
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
                var unarchivedName = cacheManager.NamingProvider.GetNameForPackageUnarchived(package);
                if (cacheManager.Contains(unarchivedName)) {
                    Console.WriteLine($"Package {package.PackageName} already cached.");
                    continue;
                }
                Console.WriteLine($"Downloading package {package.PackageName} to cache...");
                if (package.Hashes == null || package.Hashes.Count == 0) {
                    continue;
                }
                var packageArchive = hashRepository.RetrieveArchive(package.PackageName, package.Hashes).WaitAndUnwrapException();
                cacheManager.Store(unarchivedName, packageArchive);
            }

            var packageExtractor = new MPM.Archival.PackageExtractor(cacheManager);
            foreach (var package in configuration.Packages) {
                if (package.Installation != null && package.Installation.Count > 0) {
                    continue;
                }

                var unarchivedName = cacheManager.NamingProvider.GetNameForPackageUnarchived(package);

                //Load installation scripts from embedded package.json files in downloaded packages
                using (var zip = new StreamingZipFetcher(cacheManager.Fetch(unarchivedName).FetchStream())) {
                    package.Installation = JsonConvert.DeserializeObject<Build>(Encoding.UTF8.GetString(zip.FetchFile("package.json"))).Installation;
                }
            }

            //Generate operation listings for installation
            var fileMap = configuration.GenerateFileMap(cacheManager, protocolResolver);

            //Prep operation plan
            var targets = fileMap.Select(e => new {
                Path = e.Key,
                Operation = e.Value.LastOrDefault(),//Select the last operation at any path
            });

            Console.Write("Performing installation operations... ");

            var duration = TimerUtil.Time(() => {
                using (var cp = new ConsoleProgress()) {
                    var targCount = targets.Count();
                    var curTarget = 0;
                    //Perform operations
                    foreach (var target in targets) {
                        cp.Report(((double) (++curTarget)) / ((double) targCount));
                        target.Operation.Perform(fileSystem, target.Path, cacheManager);
                    }
                }
            });

            Console.WriteLine("Done in {0:0.00}s.", duration.TotalSeconds);

            instance.InstalledConfiguration = configuration;
        }
    }
}
