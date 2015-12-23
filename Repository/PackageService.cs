using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MPM.Data.Repository;
using MPM.Extensions;
using MPM.Net;
using MPM.Net.DTO.Repository.Packages;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Web;
using NServiceKit.ServiceInterface;

namespace Repository {
    public class PackageService : Service {
        //Injected by IOC
        public LiteDbPackageRepository Repository { get; set; }

        public object Get(BuildListRequest request) {
            List<MPM.Net.DTO.Build> retVal;
            //return request.Ids.IsEmpty()
            //    ? Repository.GetAll()
            //    : Repository.GetByIds(request.Ids);
            request.PackageNames = request.PackageNames.Denull().ToArray();
            if (request.PackageNames.Length == 0) {
                var packageList = this.Repository.FetchPackageList();
                retVal = packageList.SelectMany(p => p.Builds).Select(b => b.ToDTO()).ToList();
                return retVal;
            }
            retVal = request.PackageNames
                .Select(name => this.Get(new BuildRequest(name)))
                .Cast<MPM.Types.Build>()
                .Select(t => t.ToDTO())
                .ToList();
            return retVal;
        }

        private object Get(BuildRequest request) {
            if (String.IsNullOrWhiteSpace(request.PackageName)) {
                return null;
            }
            var pkg = this.Repository.FetchPackage(request.PackageName);
            var builds = pkg?.Builds;
            return (List<MPM.Net.DTO.Build>)builds.Denull().Select(b => b.ToDTO()).ToList();
        }

        public object Post(BuildSubmission newBuild) {
            var buildInfo = newBuild?.BuildInfo?.FromDTO();
            if (buildInfo == null) {
                return null;
            }
            var pkg = this.Repository.FetchPackage(newBuild.PackageName);
            if (pkg == null) {
                pkg = this.Repository.RegisterPackage(newBuild.BuildInfo.Package, buildInfo.Authors);
            }
            return this.Repository.RegisterBuild(buildInfo);
        }

        public object Put(BuildSubmission newBuild) {
            //return Repository.Store(todo);
            throw new NotImplementedException();
        }

        public void Delete(BuildRequest request) {
            if (request.PackageName.IsEmpty()) {
                throw HttpError.NotFound("No package specified to find.");
            }
            Repository.DeletePackage(request.PackageName);
        }
    }
}
