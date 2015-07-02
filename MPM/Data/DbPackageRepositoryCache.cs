using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Net.DTO;
using semver.tools;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;

namespace MPM.Data {
	class SyncInfo {
		public DateTime? LastUpdated { get; set; }
	}
	public class DbPackageRepositoryCache : IPackageRepositoryCache, IDisposable {
		/// <summary>
		/// Optionally-disposable, 
		/// </summary>
		private IPackageRepository repository;
		/// <summary>
		/// Whether or not the instance owns <see cref="repository"/> and must dispose of it if possible.
		/// </summary>
		private bool ownsRepositoryInstance;
		private IUntypedKeyValueStore<String> packageCacheDb;
		private IMetaDataManager metaDb;

		/// <param name="packageCacheDbFactory">Factory to fetch a package-cache database connection which may be disposed after usage.</param>
		/// <param name="metaDbFactory">Factory to fetch a meta database connection which may be disposed after usage.</param>
		/// <param name="repository">The repository that will be cached</param>
		/// <param name="ownsRepositoryInstance">Whether or not this instance is responsible for disposal of the <paramref name="repository"/> instance</param>
		public DbPackageRepositoryCache(IUntypedKeyValueStore<String> packageCacheDb, IMetaDataManager metaDb, IPackageRepository repository, bool ownsRepositoryInstance = false) {
			this.repository = repository;
			this.ownsRepositoryInstance = ownsRepositoryInstance;
			this.packageCacheDb = packageCacheDb;
			this.metaDb = metaDb;
		}

		public async Task<Build> FetchBuild(string packageName, SemanticVersion version, PackageSide side, string arch, string platform) {
			var package = await FetchBuilds(packageName, new VersionSpec(version));
			var build = package
				.Builds
				.Where(b => b.Version == version && b.Arch == arch && b.Platform == platform && (b.Side == side || b.Side == PackageSide.Universal))
				.OrderByDescending(b => b.Version)//Prefer higher versions
				.ThenByDescending(b => b.Side != PackageSide.Universal)//Prefer side-specific
				.FirstOrDefault();
			return build;
		}

		public async Task<Package> FetchBuilds(string packageName, VersionSpec versionSpec) {
			var package = await FetchPackage(packageName);
			var builds = package
				.Builds
				.Where(b => versionSpec.Satisfies(b.Version))
				.OrderByDescending(b => b.Version)//Prefer higher versions
				.ThenByDescending(b => b.Side != PackageSide.Universal)//Prefer side-specific
				.ToArray();
			return new Package {
				Name = package.Name,
				Authors = package.Authors,
				Builds = builds,
			};
		}

		public async Task<Package> FetchPackage(string packageName) {
			return await Task.Run<Package>(() => {
				var package = packageCacheDb.Get<Package>(packageName);
				if (package == null) {
					return null;
				}
				var builds = package
					.Builds
					.OrderByDescending(b => b.Version)//Prefer higher versions
					.ThenByDescending(b => b.Side != PackageSide.Universal)//Prefer side-specific
					.ToArray();

				return new Package {
					Name = package.Name,
					Authors = package.Authors,
					Builds = builds,
				};
			});
		}

		public async Task<IEnumerable<Package>> FetchPackageList() {
			return await Task.Run<Package[]>(() => {
				return packageCacheDb.Pairs.Cast<Package>().Where(p => p != null).ToArray();
			});
		}

		public Task<IEnumerable<Package>> FetchPackageList(DateTime updatedAfter) {
			return FetchPackageList();//Completely disregard updatedAfter constraint, as is allowed by the specification
		}

		const string SyncInfoMetaName = "syncInfo";
		public async Task Sync() {
			
			var syncInfo = metaDb.Get<SyncInfo>(SyncInfoMetaName);
			DateTime? lastUpdatedTime = syncInfo.LastUpdated;
			Package[] packageListToSync;
			if (lastUpdatedTime.HasValue) {
				packageListToSync = (await repository.FetchPackageList(lastUpdatedTime.Value)).ToArray();
			} else {
				packageListToSync = (await repository.FetchPackageList()).ToArray();
			}

			foreach (var packageInfo in packageListToSync) {
				var package = await repository.FetchPackage(packageInfo.Name);
				await UpsertPackage(package);
			}

			syncInfo.LastUpdated = DateTime.UtcNow;
			metaDb.Set<SyncInfo>(SyncInfoMetaName, syncInfo);
		}

		public void UpsertBuild(string packageName, Build build) {
			throw new NotImplementedException();
		}

		public async Task UpsertPackage(Package package) {
			await Task.Run(() => {
				packageCacheDb.Set(package.Name, package);
			});
		}

		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (packageCacheDb != null) {
					packageCacheDb = null;
					packageCacheDb.Dispose();
				}
				if (metaDb != null) {
					metaDb = null;
					metaDb.Dispose();
				}
				if (repository != null) {
					if (ownsRepositoryInstance) {
						var disposableRepository = repository as IDisposable;
						if (disposableRepository != null) {
							disposableRepository.Dispose();
							disposableRepository = null;
						}
					}
					repository = null;
				}
			}
		}
	}
}
