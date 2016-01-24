using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiteDB;

namespace MPM.Data.Repository {
    public class RepositoryServer : IDisposable {
        public readonly LiteDatabase Database;
        public readonly LiteDbHashRepository Hashes;
        public readonly LiteDbPackageRepository Packages;

        public RepositoryServer(string dataPath) {
            if (dataPath == null) {
                throw new ArgumentNullException(nameof(dataPath));
            }
            this.Database = new LiteDatabase($"filename={dataPath}; journal=false");
            this.Database.Shrink();
            //Database.Mapper.Entity<LiteDbPackageRepository.PackageRepositoryEntry>().DbRef(entry => entry.Builds, "builds");//Must add .Include(p => p.Builds) to all appropriate usages
            this.Packages = new LiteDbPackageRepository(Database.GetCollection<LiteDbPackageRepository.PackageRepositoryEntry>("packages"));
            this.Hashes = new LiteDbHashRepository(Database.FileStorage);
        }

        public void Dispose() {
            Database.Dispose();
        }
    }
}
