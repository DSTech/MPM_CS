using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;
using MPM.Types;
using MPM.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MPM.Data.Repository;

namespace MPM.Data {
    public class LiteDbPackageRepositoryCache : IPackageRepositoryCache {
        private const string SyncInfoMetaName = "syncInfo";
        private readonly IMetaDataManager metaDb;

        private readonly LiteCollection<PackageEntry> Packages;
        private readonly IPackageRepository repository;


        /// <param name="packageCacheDbFactory">
        ///     Factory to fetch a package-cache database connection which may be disposed after
        ///     usage.
        /// </param>
        /// <param name="metaDb">Metadata Manager</param>
        /// <param name="repository">The repository that will be cached</param>
        public LiteDbPackageRepositoryCache(LiteCollection<PackageEntry> packages, IMetaDataManager metaDb, IPackageRepository repository) {
            if ((this.Packages = packages) == null) {
                throw new ArgumentNullException(nameof(packages));
            }
            this.metaDb = metaDb;
            this.repository = repository;
        }

        public Build FetchBuild(string packageName, SemanticVersion fetchVersion, CompatibilitySide fetchSide, Arch fetchArch) {
            var builds = this.FetchBuilds(packageName, new VersionSpec(fetchVersion));
            var build = builds
                .Where(b => true
                    && b.Version == fetchVersion
                    && b.Arch == fetchArch
                    && (b.Side == fetchSide || b.Side == CompatibilitySide.Universal))
                .OrderByDescending(b => b.Side != CompatibilitySide.Universal)//Prefer side-specific
                .FirstOrDefault();
            return build;
        }

        public IEnumerable<Build> FetchBuilds(string packageName, VersionSpec versionSpec) {
            var package = this.FetchPackage(packageName);
            var builds = package
                .Builds
                .Where(b => versionSpec.Satisfies(b.Version))
                .OrderByDescending(b => b.Version)//Prefer higher versions
                .ThenByDescending(b => b.Side != CompatibilitySide.Universal)//Prefer side-specific
                .ToArray();
            return builds;
        }

        public Package FetchPackage(string packageName) {
            var package = (Package) Packages.FindById(packageName);
            if (package == null) {
                return null;
            }
            var builds = package
                .Builds
                .OrderByDescending(b => b.Version)//Prefer higher versions
                .ThenByDescending(b => b.Side != CompatibilitySide.Universal)//Prefer side-specific
                .ToArray();

            return new Package(package.Name, package.Authors, builds);
        }

        public IEnumerable<Package> FetchPackageList() {
            return Packages.FindAll().Cast<Package>().ToArray();
        }

        public IEnumerable<Package> FetchPackageList(DateTime updatedAfter) {
            return this.FetchPackageList();//Completely disregard updatedAfter constraint, as is allowed by the specification
        }

        public void Sync() {
            var syncInfo = metaDb.Get<SyncInfo>(SyncInfoMetaName);
            DateTime? lastUpdatedTime = syncInfo.LastUpdated;
            IEnumerable<Package> packageListToSync;
            if (lastUpdatedTime.HasValue) {
                packageListToSync = repository.FetchPackageList(lastUpdatedTime.Value);
            } else {
                packageListToSync = repository.FetchPackageList();
            }

            foreach (var packageInfo in packageListToSync.ToArray()) {
                var package = repository.FetchPackage(packageInfo.Name);
                UpsertPackage(package);
            }

            syncInfo.LastUpdated = DateTime.UtcNow;
            metaDb.Set<SyncInfo>(SyncInfoMetaName, syncInfo);
        }

        public void UpsertBuild(string packageName, Build build) {
            throw new NotImplementedException();
        }

        public void UpsertPackage(Package package) {
            Packages.Upsert(package);
        }

        public class PackageEntry {
            public PackageEntry() {
            }

            public PackageEntry(Package package) {
                this.Name = package.Name;
                this.Authors = package.Authors.Select(DTOTranslationX.ToDTO).ToList();
                this.Builds = package.Builds.Select(DTOTranslationX.ToDTO).ToList();
            }

            [BsonId]
            public String Name { get; set; }

            [BsonField]
            public List<Net.DTO.Author> Authors { get; set; }

            [BsonField]
            public List<Net.DTO.Build> Builds { get; set; }

            public static implicit operator PackageEntry(Package package) {
                return new PackageEntry(package);
            }

            public static implicit operator Package(PackageEntry entry) {
                return new Package(
                    entry.Name,
                    entry.Authors.Select(DTOTranslationX.FromDTO).ToArray(),
                    entry.Builds.Select(DTOTranslationX.FromDTO).ToArray()
                    );
            }

            public static implicit operator Net.DTO.Package(PackageEntry entry) {
                return new Net.DTO.Package {
                    Name = entry.Name,
                    Authors = entry.Authors,
                    Builds = entry.Builds,
                };
            }
        }
    }
}
