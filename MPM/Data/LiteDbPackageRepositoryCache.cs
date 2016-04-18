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
using MPM.Util;

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

        private static readonly TimeSpan SyncPeriod = TimeSpan.FromMinutes(5);
        public void Sync() {
            var lastSynced = GetLastSynced();

            //Skip if synced in the last sync period
            if (lastSynced > DateTime.UtcNow.Subtract(SyncPeriod)) {
                return;
            }

            Console.Write("Package cache older than one minute- ");
            using (new ConsoleIndenter()) {
                var syncDuration = TimerUtil.Time(() => {
                    IEnumerable<Build> builds;
                    if (lastSynced == DateTime.MinValue) {
                        Console.Write("Updating via full fetch... ");
                        var duration = TimerUtil.Time(out builds, () => {
                            using (var consoleProgress = new ConsoleProgress()) {
                                builds = repository.FetchBuilds().ToArray();
                            }
                        });
                        Console.WriteLine("Loaded in {0:0.00}s.", duration.TotalSeconds);
                    } else {
                        Console.Write("Updating via delta fetch... ");
                        var duration = TimerUtil.Time(out builds, () => {
                            using (var consoleProgress = new ConsoleProgress()) {
                                builds = repository.FetchBuilds(lastSynced).ToArray();
                            }
                        });
                        Console.WriteLine("Loaded in {0:0.00}s.", duration.TotalSeconds);
                    }
                    foreach (var build in builds) {
                        var found = Builds.FetchBuild(build);
                        if (found != null) {
                            Console.Write($"Overwriting build {build.ToIdentifierString()}");
                            Builds.UpdateBuild(build);
                        } else {
                            Console.WriteLine($"Registering build {build.ToIdentifierString()}");
                            Builds.RegisterBuild(build);
                        }
                    }
                });
                Console.WriteLine("Synced in {0:0.00}s.", syncDuration.TotalSeconds);
            }
            SetLastSynced(DateTime.UtcNow);
        }

        #endregion
    }
}
