using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Net.DTO;
using semver.tools;
using System.IO;
using Couchbase;
using Couchbase.Lite;
using Couchbase.Lite.Storage;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MPM.Data {
	public static class CouchbaseJsonExtensions {
		private static object CouchifyToken(JToken token) {
			switch (token.Type) {
				case JTokenType.Object:
					return (token as JObject).ToCouch();
				case JTokenType.Array:
					return (token as JArray).Select(val => CouchifyToken(val)).ToArray();
				default:
					return token.ToString(Formatting.None);
			}
		}
		public static IDictionary<string, object> ToCouch(this JObject obj) {
			var res = new Dictionary<string, object>();
			foreach (var property in obj.Properties()) {
				res.Add(property.Name, CouchifyToken(property.Value));
			}
			return res;
		}
		public static JObject FromCouch(this IDictionary<string, object> obj) => JObject.FromObject(obj);
	}
	public class CouchbaseLitePackageRepositoryCache : IPackageRepositoryCache, IDisposable {
		private IPackageRepository repository;
		private bool ownsRepositoryInstance;
		private Couchbase.Lite.Manager db;

		/// <param name="repository">The repository that will be cached</param>
		/// <param name="dbLocation">The location wherein the cache will be (or is already) stored</param>
		/// <param name="ownsRepositoryInstance">Whether or not this instance is responsible for disposal of the <paramref name="repository"/> instance</param>
		public CouchbaseLitePackageRepositoryCache(IPackageRepository repository, Uri dbLocation, bool ownsRepositoryInstance = false) {
			this.repository = repository;
			this.ownsRepositoryInstance = ownsRepositoryInstance;
			this.db = new Manager(Directory.CreateDirectory(dbLocation.AbsolutePath), new ManagerOptions());
			PrepDatabase(db);
		}

		/// <summary>
		/// Creates and migrates tables to be used in other methods.
		/// </summary>
		/// <param name="db"></param>
		private void PrepDatabase(Manager db) {
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
				var packages = db.GetDatabase("packages");
				var packageDoc = packages.GetExistingDocument(packageName);
				if (packageDoc == null) {
					return null;
				}
				var package = packageDoc.UserProperties.FromCouch().ToObject<Package>();
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
				var packages = db.GetDatabase("packages");
				return packages.CreateAllDocumentsQuery()
					.Run()
					.Select(row => row.Document)
					.Select(
						packageDoc => packageDoc
							.UserProperties
							.FromCouch()
							.ToObject<Package>()
					)
					.ToArray();
			});
		}

		public Task<IEnumerable<Package>> FetchPackageList(DateTime updatedAfter) {
			return FetchPackageList();//Completely disregard updatedAfter constraint, as is allowed by the specification
		}

		public async Task Sync() {
			var meta = db.GetDatabase("meta");
			var syncInfo = meta.GetDocument("syncInfo");
			DateTime? lastUpdatedTime = syncInfo.GetProperty<DateTime?>("lastUpdated");
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
			syncInfo.PutProperties(new Dictionary<string, object> {
				["lastUpdated"] = DateTime.UtcNow,
			});
		}

		public void UpsertBuild(string packageName, Build build) {
			throw new NotImplementedException();
		}

		public async Task UpsertPackage(Package package) {
			await Task.Run(() => {
				var packages = db.GetDatabase("packages");
				var packageDoc = packages.GetDocument(package.Name);
				if (packageDoc == null) {
					packageDoc = packages.CreateDocument();
					packageDoc.Id = package.Name;
				}
				packageDoc.PutProperties(JObject.FromObject(package).ToCouch());
			});
		}

		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (db != null) {
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
