using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using MPM.Data.Repository;
using MPM.Extensions;
using MPM.Net;
using MPM.Types;
using Newtonsoft.Json;
using NServiceKit.Common.Extensions;
using NServiceKit.Common.Web;
using NServiceKit.ServiceInterface;
using Repository.Types.Packages;

namespace Repository {
    public class PackageService : Service {
        //Injected by IOC
        public LiteDbPackageRepository Repository { get; set; }

        public object Get(BuildListRequest request) {
            IEnumerable<Build> buildList;
            if (request.UpdatedAfter.HasValue) {
                buildList = this.Repository.FetchBuilds(request.UpdatedAfter.Value);
            } else {
                buildList = this.Repository.FetchBuilds();
            }
            return (List<MPM.Types.Build>)buildList
                    .OrderByDescending(b => b.Arch)
                    .ThenByDescending(b => b.Version)
                    .ThenByDescending(b => b.Side != MPM.Types.CompatibilitySide.Universal)
                    .Take(200)
                    .ToList();
        }

        [Authenticate]
        public object Post(BuildSubmission submission) {
            var body = Request.GetRawBody();
            var buildInfo = JsonConvert.DeserializeObject<Build>(body);
            if (buildInfo == null) {
                return null;
            }
            if (buildInfo.PackageName.IsNullOrWhiteSpace()) {
                return null;
            }
            var existingBuilds = this.Repository.FetchBuilds()
                .Where(buildInfo.IdentityMatch)
                .ToArray()
                ;
            if (existingBuilds.Length >= 1) {
                return this.Repository.UpdateBuild(buildInfo);
            } else {
                return this.Repository.RegisterBuild(buildInfo);
            }
        }

        [Authenticate]
        public object Delete(BuildDeletionRequest request) {
            var body = Request.GetRawBody();
            var anonObj = new {
                name = (String)null,
                version = (SemVersion)null,
                arch = (Arch)null,
                side = (CompatibilitySide)default(CompatibilitySide),
            };
            var pseudoBuild = JsonConvert.DeserializeAnonymousType(body, anonObj);
            var buildInfo = new Build();
            buildInfo.PackageName = pseudoBuild.name;
            buildInfo.Arch = pseudoBuild.arch;
            buildInfo.Version = pseudoBuild.version;
            buildInfo.Side = pseudoBuild.side;
            if (buildInfo == null) {
                throw HttpError.NotFound("No build specified to delete.");
            }
            return this.Repository.DeleteBuild(buildInfo);
        }
    }
}
