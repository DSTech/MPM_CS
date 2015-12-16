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
using MPM.Data.Repository;

namespace MPM.Data {

	public class LiteDbPackageRepositoryCache : IPackageRepositoryCache {
		public class PackageEntry {
			public PackageEntry() { }
			public PackageEntry(Package package) {
				this.Name = package.Name;
				this.Authors = package.Authors.Select(DTOTranslationExtensions.ToDTO).ToList();
				this.Builds = package.Builds.Select(DTOTranslationExtensions.ToDTO).ToList();
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
		private readonly IPackageRepository repository;

		private readonly LiteCollection<PackageEntry> Packages;
		private readonly IMetaDataManager metaDb;


		/// <param name="packageCacheDbFactory">Factory to fetch a package-cache database connection which may be disposed after usage.</param>
		/// <param name="metaDb">Metadata Manager</param>
		/// <param name="repository">The repository that will be cached</param>
		public LiteDbPackageRepositoryCache(LiteCollection<PackageEntry> packages, IMetaDataManager metaDb, IPackageRepository repository) {
			if ((this.Packages = packages) == null) {
				throw new ArgumentNullException(nameof(packages));
			}
			this.metaDb = metaDb;
			this.repository = repository;
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
	}
}