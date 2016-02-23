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

        private readonly LiteDbPackageRepository Builds;
        private readonly IPackageRepository repository;


        /// <param name="packageCacheDbFactory">
        ///     Factory to fetch a package-cache database connection which may be disposed after
        ///     usage.
        /// </param>
        /// <param name="metaDb">Metadata Manager</param>
        /// <param name="repository">The repository that will be cached</param>
        public LiteDbPackageRepositoryCache(LiteDbPackageRepository packageRepo, IMetaDataManager metaDb, IPackageRepository repository) {
            if ((this.Builds = packageRepo) == null) {
                throw new ArgumentNullException(nameof(packageRepo));
            }
            this.metaDb = metaDb;
            this.repository = repository;
        }

        #region Implementation of IPackageRepository

        public IEnumerable<Build> FetchBuilds() {
            Sync();
            return Builds.FetchBuilds();
        }

        public IEnumerable<Build> FetchBuilds(DateTime updatedAfter) {
            Sync();
            return Builds.FetchBuilds(updatedAfter);
        }

        private DateTime GetLastSynced() {
            string lastSynced;
            lock (this) {
                lastSynced = metaDb.Get<string>("lastSynced");
            }
            if (lastSynced == null) {
                return DateTime.MinValue;
            }
            return JsonConvert.DeserializeObject<DateTime>(lastSynced);
        }

        private void SetLastSynced(DateTime lastSynced) {
            lock (this) {
                metaDb.Set("lastSynced", JsonConvert.SerializeObject(lastSynced));
            }
        }

        public void Sync() {
            var lastSynced = GetLastSynced();
            if (lastSynced > DateTime.UtcNow/*.AddMinutes(-1.0)*/) {
                return;
            }
            Console.Write("Package cache older than one minute- ");
            IEnumerable<Build> builds;
            if (lastSynced == DateTime.MinValue) {
                Console.WriteLine("Updating via full fetch...");
                builds = repository.FetchBuilds();
            } else {
                Console.WriteLine("Updating via delta fetch...");
                builds = repository.FetchBuilds(lastSynced);
            }
            foreach (var build in builds) {
                var found = Builds.FetchBuild(build);
                if (found != null) {
                    Console.WriteLine($"\tOverwriting build {build.ToIdentifierString()}");
                    Builds.UpdateBuild(build);
                } else {
                    Console.WriteLine($"\tRegistering build {build.ToIdentifierString()}");
                    Builds.RegisterBuild(build);
                }
            }
            Console.WriteLine("Synced.");
            SetLastSynced(DateTime.UtcNow);
        }

        #endregion
    }
}
