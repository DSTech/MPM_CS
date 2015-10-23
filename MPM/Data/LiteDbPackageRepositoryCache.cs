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
using semver.tools;

namespace MPM.Data {

	public class LiteDbPackageRepositoryCache : IPackageRepositoryCache, IDisposable {
		public class PackageEntry {
			public PackageEntry() { }
			public PackageEntry(Package package) {
				this.Name = package.Name;
				this.Authors = package.Authors.Select(DTOTranslationExtensions.ToDTO).ToArray();
				this.Builds = package.Builds.Select(DTOTranslationExtensions.ToDTO).ToArray();
			}

			[BsonId]
			public String Name { get; set; }
			public Net.DTO.Author[] Authors { get; set; }
			public Net.DTO.Build[] Builds { get; set; }

			public static implicit operator PackageEntry(Package package) {
				return new PackageEntry(package);
			}
			public static implicit operator Package(PackageEntry entry) {
				return new Package(
					entry.Name,
					entry.Authors.Select(DTOTranslationExtensions.FromDTO).ToArray(),
					entry.Builds.Select(DTOTranslationExtensions.FromDTO).ToArray()
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

		private const string SyncInfoMetaName = "syncInfo";

		/// <summary>
		/// Optionally-disposable,
		/// </summary>
		private readonly IPackageRepository repository;

		/// <summary>
		/// Whether or not the instance owns <see cref="repository"/> and must dispose of it if possible.
		/// </summary>
		private readonly bool ownsRepositoryInstance;

		private readonly bool ownsMetaInstance;

		private readonly LiteCollection<PackageEntry> Packages;
		private readonly IMetaDataManager metaDb;


		/// <param name="packageCacheDbFactory">Factory to fetch a package-cache database connection which may be disposed after usage.</param>
		/// <param name="metaDbFactory">Factory to fetch a meta database connection which may be disposed after usage.</param>
		/// <param name="repository">The repository that will be cached</param>
		/// <param name="ownsRepositoryInstance">Whether or not this instance is responsible for disposal of the <paramref name="repository"/> instance</param>
		public LiteDbPackageRepositoryCache(LiteDB.LiteCollection<PackageEntry> packages, IMetaDataManager metaDb, IPackageRepository repository, bool ownsMetaInstance = false, bool ownsRepositoryInstance = false) {
			this.Packages = packages;
			this.metaDb = metaDb;
			this.repository = repository;
			this.ownsMetaInstance = ownsMetaInstance;
			this.ownsRepositoryInstance = ownsRepositoryInstance;
		}

		public async Task<Build> FetchBuild(string packageName, SemanticVersion version, CompatibilitySide side, Arch arch, CompatibilityPlatform platform) {
			var builds = await FetchBuilds(packageName, new VersionSpec(version));
			var build = builds
				.Where(b => b.Version == version && b.Arch == arch && b.Platform == platform && (b.Side == side || b.Side == CompatibilitySide.Universal))
				.OrderByDescending(b => b.Version)//Prefer higher versions
				.ThenByDescending(b => b.Side != CompatibilitySide.Universal)//Prefer side-specific
				.FirstOrDefault();
			return build;
		}

		public async Task<IEnumerable<Build>> FetchBuilds(string packageName, VersionSpec versionSpec) {
			var package = await FetchPackage(packageName);
			var builds = package
				.Builds
				.Where(b => versionSpec.Satisfies(b.Version))
				.OrderByDescending(b => b.Version)//Prefer higher versions
				.ThenByDescending(b => b.Side != CompatibilitySide.Universal)//Prefer side-specific
				.ToArray();
			return builds;
		}

		public async Task<Package> FetchPackage(string packageName) {
			return await Task.Run<Package>(() => {
				var package = (Package)Packages.FindById(packageName);
				if (package == null) {
					return null;
				}
				var builds = package
					.Builds
					.OrderByDescending(b => b.Version)//Prefer higher versions
					.ThenByDescending(b => b.Side != CompatibilitySide.Universal)//Prefer side-specific
					.ToArray();

				return new Package(package.Name, package.Authors, builds);
			});
		}

		public async Task<IEnumerable<Package>> FetchPackageList() {
			return await Task.Run<Package[]>(() => {
				return Packages.FindAll().Cast<Package>().ToArray();
			});
		}

		public Task<IEnumerable<Package>> FetchPackageList(DateTime updatedAfter) {
			return FetchPackageList();//Completely disregard updatedAfter constraint, as is allowed by the specification
		}

		public async Task Sync() {
			var syncInfo = metaDb.Get<SyncInfo>(SyncInfoMetaName);
			DateTime? lastUpdatedTime = syncInfo.LastUpdated;
			IEnumerable<Package> packageListToSync;
			if (lastUpdatedTime.HasValue) {
				packageListToSync = await repository.FetchPackageList(lastUpdatedTime.Value);
			} else {
				packageListToSync = await repository.FetchPackageList();
			}

			foreach (var packageInfo in packageListToSync.ToArray()) {
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
				Packages.Upsert(package);
			});
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (metaDb != null) {
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
				}
			}
		}
	}
}
