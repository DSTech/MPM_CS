using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using MPM.Extensions;
using MPM.Types;
using Newtonsoft.Json;

namespace MPM.Data.Repository {
    public class HttpPackageRepository : IPackageRepository {
        private readonly Uri baseUri;

        public HttpPackageRepository(Uri baseUri) {
            this.baseUri = baseUri;
        }

        public Build FetchBuild(string packageName, SemVer.Version version, CompatibilitySide side, Arch arch) {
            var req = WebRequest.CreateHttp(new Uri(baseUri, $"/packages/{arch}/{side}/{packageName}/{version}"));
            byte[] responseData;
            using (var response = req.GetResponse()) {
                responseData = response.GetResponseStream()?.ReadToEndAndClose();
            }
            var build = JsonConvert.DeserializeObject<Build>(Encoding.UTF8.GetString(responseData));
            build.PackageName = build.PackageName ?? packageName;
            return build;
        }

        public IEnumerable<Build> FetchBuilds(string packageName, SemVer.Range versionSpec) {
            var package = this.FetchPackageBuilds(packageName);
            var matchingBuilds = package
                .Where(b => versionSpec.IsSatisfied(b.Version))
                .ToArray();
            return matchingBuilds;
        }

        public IEnumerable<Build> FetchPackageBuilds(string packageName) {
            var req = WebRequest.CreateHttp(new Uri(baseUri, $"/packages/{packageName}"));
            byte[] responseData;
            using (var response = req.GetResponse()) {
                responseData = response.GetResponseStream().ReadToEndAndClose();
            }
            var builds = JsonConvert.DeserializeObject<IEnumerable<Build>>(Encoding.UTF8.GetString(responseData));
            return builds;
        }

        public IEnumerable<Build> FetchPackageList() => _FetchPackageList(null);
        public IEnumerable<Build> FetchPackageList(DateTime updatedAfter) => _FetchPackageList(updatedAfter);
        private IEnumerable<Build> _FetchPackageList(DateTime? updatedAfter) {
            var lastSyncClause = "";
            if(updatedAfter.HasValue) {
                var updatedAfterTimestamp = (long)(updatedAfter.Value.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                lastSyncClause = $"?LastSync={updatedAfterTimestamp}";
            }
            var req = WebRequest.CreateHttp(new Uri(baseUri, $"/packages/{lastSyncClause}"));
            byte[] responseData;
            using (var response = req.GetResponse()) {
                responseData = response.GetResponseStream().ReadToEndAndClose();
            }
            var builds = JsonConvert.DeserializeObject<IEnumerable<Build>>(Encoding.UTF8.GetString(responseData));
            return builds.ToArray();
        }
    }
}
