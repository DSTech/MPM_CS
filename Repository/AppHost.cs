using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Funq;
using LiteDB;
using MPM.Data.Repository;
using MPM.Extensions;
using MPM.Net;
using MPM.Net.Protocols.Minecraft;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.WebHost.Endpoints;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;

namespace Repository {
    //Web Service Host Configuration
    public class AppHost : AppHostHttpListenerBase {
        public AppHost() : base("Package and Hash Repository", typeof(Build).Assembly) { }

        public override void Configure(Container container) {
            var liteDbPackages = new LiteDatabase($"filename={"./data/packages.ldb"}; journal=false");
            var liteDbHashes = new LiteDatabase($"filename={"./data/hashes.ldb"}; journal=false");
            container.Register<LiteDbPackageRepository>(new LiteDbPackageRepository(liteDbPackages.GetCollection<LiteDbPackageRepository.PackageRepositoryEntry>("Packages")));
            container.Register<IPackageRepository>(container.Resolve<LiteDbPackageRepository>());
            container.Register<LiteDbHashRepository>(new LiteDbHashRepository(liteDbHashes.FileStorage));
            container.Register<IHashRepository>(container.Resolve<LiteDbHashRepository>());
        }
    }

    //REST Resource DTO
    [Route("/builds")]
    [Route("/builds/{" + nameof(PackageNames) + "}")]
    public class Builds : IReturn<List<MPM.Net.DTO.Build>> {
        public string[] PackageNames { get; set; }
        public Builds(params string[] packageNames) {
            this.PackageNames = packageNames;
        }
    }

    [Route("/builds", "POST")]
    [Route("/builds/{" + nameof(PackageName) + "}", "PUT")]
    public class Build : IReturn<MPM.Net.DTO.Build> {
        public string PackageName { get; set; }
        public MPM.Net.DTO.Build BuildInfo { get; set; }
    }
    public class SinglePackage : IReturn<IList<MPM.Net.DTO.Build>> {
        public SinglePackage() {
        }
        public SinglePackage(string packageName) {
            this.PackageName = packageName;
        }
        public string PackageName { get; set; }
    }

    public class TodosService : Service {
        //Injected by IOC
        public LiteDbPackageRepository Repository { get; set; }

        public object Get(Builds request) {
            List<MPM.Net.DTO.Build> retVal;
            //return request.Ids.IsEmpty()
            //    ? Repository.GetAll()
            //    : Repository.GetByIds(request.Ids);
            request.PackageNames = request.PackageNames.Denull().ToArray();
            if (request.PackageNames.Length == 0) {
                var packageList = Repository.FetchPackageList();
                retVal = packageList.SelectMany(p => p.Builds).Select(b => b.ToDTO()).ToList();
                return retVal;
            }
            retVal = request.PackageNames
                .Select(name => Get(new SinglePackage(name)))
                .Cast<MPM.Types.Build>()
                .Select(t => t.ToDTO())
                .ToList();
            return retVal;
        }

        private object Get(SinglePackage request) {
            if (String.IsNullOrWhiteSpace(request.PackageName)) {
                return null;
            }
            var pkg = Repository.FetchPackage(request.PackageName);
            var builds = pkg?.Builds;
            return (List<MPM.Net.DTO.Build>)builds.Denull().Select(b => b.ToDTO()).ToList();
        }

        public object Post(Build build) {
            var buildInfo = build?.BuildInfo?.FromDTO();
            if (buildInfo == null) {
                return null;
            }
            var pkg = Repository.FetchPackage(build.PackageName);
            if (pkg == null) {
                pkg = Repository.RegisterPackage(build.BuildInfo.Package, buildInfo.Authors);
            }
            return Repository.RegisterBuild(buildInfo);
        }

        public object Put(Build build) {
            //return Repository.Store(todo);
            throw new NotImplementedException();
        }

        //public void Delete(Builds request) {
        //    //Repository.DeleteByIds(request.Ids);
        //}
    }
}
