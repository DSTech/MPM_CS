using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Net.DTO;
using semver.tools;
using System.Data.SQLite;
using System.Data.SQLite.Linq;
using System.Data.SQLite.Generic;
using System.IO;

namespace MPM.Data {
	public class SqlitePackageRepositoryCache : IPackageRepositoryCache, IDisposable {
		private IPackageRepository repository;
		private bool ownsRepositoryInstance;
		private SQLiteConnection db;

		/// <param name="repository">The repository that will be cached</param>
		/// <param name="sqliteDbLocation">The location wherein the cache will be (or is already) stored</param>
		/// <param name="ownsRepositoryInstance">Whether or not this instance is responsible for disposal of the <paramref name="repository"/> instance</param>
		public SqlitePackageRepositoryCache(IPackageRepository repository, Uri sqliteDbLocation, bool ownsRepositoryInstance = false) {
			this.repository = repository;
			this.ownsRepositoryInstance = ownsRepositoryInstance;
			var strBldr = new SQLiteConnectionStringBuilder();
			if(!File.Exists(sqliteDbLocation.AbsolutePath)) {
				SQLiteConnection.CreateFile(sqliteDbLocation.AbsolutePath);
			}
			strBldr.DataSource = sqliteDbLocation.AbsolutePath;
			this.db = new System.Data.SQLite.SQLiteConnection(strBldr.ConnectionString);

			PrepDatabase(db);
		}

		/// <summary>
		/// Creates and migrates tables to be used in other methods.
		/// </summary>
		/// <param name="db"></param>
		private void PrepDatabase(SQLiteConnection db) {
			//using (var cmd = db.CreateCommand()) {
			//	cmd.CommandText = "CREATE TABLE IF NOT EXISTS ...";
			//}
			throw new NotImplementedException();
		}

		public Task<Build> FetchBuild(string packageName, SemanticVersion version, PackageSide side, string arch, string platform) {
			throw new NotImplementedException();
		}

		public Task<Package> FetchBuilds(string packageName, VersionSpec versionSpec) {
			throw new NotImplementedException();
		}

		public Task<Package> FetchPackage(string packageName) {
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Package>> FetchPackageList() {
			throw new NotImplementedException();
		}

		public Task<IEnumerable<Package>> FetchPackageList(DateTime updatedAfter) {
			throw new NotImplementedException();
		}

		public Task Sync() {
			throw new NotImplementedException();
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
