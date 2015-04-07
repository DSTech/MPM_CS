using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Net.DTO;
using semver.tools;
using System.IO;
using DBreeze;
using DBreeze.Storage;
using DBreeze.Utils;
using DBreeze.Utils.Async;

namespace MPM.Data {
	public class DBreezePackageRepositoryCache : IPackageRepositoryCache, IDisposable {
		private IPackageRepository repository;
		private bool ownsRepositoryInstance;
		private DBreezeEngine db;

		/// <param name="repository">The repository that will be cached</param>
		/// <param name="dbLocation">The location wherein the cache will be (or is already) stored</param>
		/// <param name="ownsRepositoryInstance">Whether or not this instance is responsible for disposal of the <paramref name="repository"/> instance</param>
		public DBreezePackageRepositoryCache(IPackageRepository repository, Uri dbLocation, bool ownsRepositoryInstance = false) {
			this.repository = repository;
			this.ownsRepositoryInstance = ownsRepositoryInstance;
			{
				var dbConf = new DBreezeConfiguration {
					DBreezeDataFolderName = dbLocation.AbsolutePath,
				};
				this.db = new DBreezeEngine(dbConf);

				//This library has the worst naming conventions on the planet
				DBreeze.Utils.CustomSerializator.Serializator = Newtonsoft.Json.JsonConvert.SerializeObject;
				DBreeze.Utils.CustomSerializator.Deserializator = Newtonsoft.Json.JsonConvert.DeserializeObject;
			}
			PrepDatabase(db);
		}

		/// <summary>
		/// Creates and migrates tables to be used in other methods.
		/// </summary>
		/// <param name="db"></param>
		private void PrepDatabase(DBreezeEngine db) {
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
				using (var trans = db.GetTransaction(eTransactionTablesLockTypes.SHARED, "packages")) {
					var selector = trans.Select<string, Package>("packages", packageName);
					if (!selector.Exists) {
						return null;
					}
					var package = selector.Value;
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
				}
			});
		}

		public async Task<IEnumerable<Package>> FetchPackageList() {
			return await Task.Run<Package[]>(() => {
				using (var trans = db.GetTransaction(eTransactionTablesLockTypes.SHARED, "packages")) {
					var selector = trans.SelectForward<string, Package>("packages");
					return selector
						.Where(row => row.Exists)
						.Select(row => row.Value)
						.ToArray();
				}
			});
		}

		public Task<IEnumerable<Package>> FetchPackageList(DateTime updatedAfter) {
			return FetchPackageList();//Completely disregard updatedAfter constraint, as is allowed by the specification
		}

		public async Task Sync() {
			DateTime? lastUpdatedTime = await Task.Run<DateTime?>(() => {
				using (var trans = db.GetTransaction(eTransactionTablesLockTypes.SHARED, "meta")) {
					var lastUpdatedSelect = trans.Select<string, DateTime>("meta", "lastUpdated");
					if (lastUpdatedSelect.Exists) {
						return null;
					}
					return lastUpdatedSelect.Value;
                }
			});
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
		}

		public void UpsertBuild(string packageName, Build build) {
			throw new NotImplementedException();
		}

		public async Task UpsertPackage(Package package) {
			await Task.Run(() => {
				using (var trans = db.GetTransaction(eTransactionTablesLockTypes.EXCLUSIVE, "packages")) {
					trans.Insert("packages", package.Name, package);
				}
			});
		}

		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (db != null) {
					db.Dispose();
					db = null;
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
