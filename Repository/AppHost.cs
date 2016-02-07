using System.IO;
using System.Text;
using System.Threading.Tasks;
using Funq;
using LiteDB;
using MPM.Data.Repository;
using MPM.Net.Protocols.Minecraft;
using NServiceKit.ServiceHost;
using NServiceKit.WebHost.Endpoints;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Repository {
    //Web Service Host Configuration
    public class AppHost : AppHostHttpListenerBase {
        public AppHost() : base("Package and Hash Repository", typeof(PackageService).Assembly) {
        }

        public override void Configure(Container container) {
            var dataDir = Directory.CreateDirectory(new DirectoryInfo("./data/").FullName).FullName;
            var packageDbPath = Path.Combine(dataDir, "packages.ldb");
            var hashDbPath = Path.Combine(dataDir, "hashes.ldb");
            var liteDbPackages = new LiteDatabase($"filename={packageDbPath}; journal=false");
            var liteDbHashes = new LiteDatabase($"filename={hashDbPath}; journal=false");
            container.Register<LiteDbPackageRepository>(new LiteDbPackageRepository(liteDbPackages.GetCollection<LiteDbPackageRepository.BuildEntry>("Builds")));
            container.Register<IPackageRepository>(container.Resolve<LiteDbPackageRepository>());
            container.Register<LiteDbHashRepository>(new LiteDbHashRepository(liteDbHashes.FileStorage));
            container.Register<IHashRepository>(container.Resolve<LiteDbHashRepository>());
        }
    }
}
