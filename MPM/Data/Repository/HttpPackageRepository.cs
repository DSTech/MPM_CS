using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;
using MPM.Net;
using MPM.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using semver.tools;

namespace MPM.Data.Repository {

    public class HttpPackageRepository : IPackageRepository {
        private readonly Uri baseUri;

        public HttpPackageRepository(Uri baseUri) {
            this.baseUri = baseUri;
        }

        public Build FetchBuild(string packageName, SemanticVersion version, CompatibilitySide side, Arch arch) {
            var req = WebRequest.CreateHttp(new Uri(baseUri, $"/packages/{arch}/{side}/{packageName}/{version}"));
            byte[] responseData;
            using (var response = req.GetResponse()) {
                responseData = response.GetResponseStream()?.ReadToEndAndClose();
            }
            var build = JsonConvert.DeserializeObject<MPM.Net.DTO.Build>(Encoding.UTF8.GetString(responseData));
            build.Package = build.Package ?? packageName;
            return build.FromDTO();
        }

        public IEnumerable<Build> FetchBuilds(string packageName, VersionSpec versionSpec) {
            var package = this.FetchPackage(packageName);
            var matchingBuilds = package
                .Builds
                .Where(b => versionSpec.Satisfies(b.Version))
                .ToArray();
            return matchingBuilds;
        }

        public Package FetchPackage(string packageName) {
            var req = WebRequest.CreateHttp(new Uri(baseUri, $"/packages/{packageName}"));
            byte[] responseData;
            using (var response = req.GetResponse()) {
                responseData = response.GetResponseStream().ReadToEndAndClose();
            }
            var package = JsonConvert.DeserializeObject<MPM.Net.DTO.Package>(Encoding.UTF8.GetString(responseData));
            return package.FromDTO();
        }

        public IEnumerable<Package> FetchPackageList() {
            var req = WebRequest.CreateHttp(new Uri(baseUri, "/packages/"));
            byte[] responseData;
            using (var response = req.GetResponse()) {
                responseData = response.GetResponseStream().ReadToEndAndClose();
            }
            var packageList = JsonConvert.DeserializeObject<IEnumerable<MPM.Net.DTO.Package>>(Encoding.UTF8.GetString(responseData));
            return packageList.Select(package => package.FromDTO()).ToArray();
        }

        public IEnumerable<Package> FetchPackageList(DateTime updatedAfter) {
            var updatedAfterTimestamp = (long)(updatedAfter.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            var req = WebRequest.CreateHttp(new Uri(baseUri, $"/packages/?LastSync={updatedAfterTimestamp}"));
            byte[] responseData;
            using (var response = req.GetResponse()) {
                responseData = response.GetResponseStream().ReadToEndAndClose();
            }
            var packageList = JsonConvert.DeserializeObject<IEnumerable<MPM.Net.DTO.Package>>(Encoding.UTF8.GetString(responseData));
            return packageList.Select(package => package.FromDTO()).ToArray();
        }
    }
}
