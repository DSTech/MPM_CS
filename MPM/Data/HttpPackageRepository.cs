using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MPM.Net.DTO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NServiceKit.Common;
using semver.tools;

namespace MPM.Data {
	public class HttpPackageRepository : IPackageRepository {
		private readonly Uri baseUri;

		public HttpPackageRepository(Uri baseUri) {
			this.baseUri = baseUri;
		}

		public async Task<Build> FetchBuild(string packageName, SemanticVersion version, PackageSide side, string arch, string platform) {
			var req = WebRequest.CreateHttp(new Uri(baseUri, $"/{packageName}/{version}"));
			var reqStream = await req.GetRequestStreamAsync();
			var res = await reqStream.ReadToEndAsync();
			var build = JsonConvert.DeserializeObject<Build>(Encoding.UTF8.GetString(res));
			return build;
		}

		public async Task<Package> FetchBuilds(string packageName, VersionSpec versionSpec) {
			var package = await FetchPackage(packageName);
			var matchingBuilds = package.Builds.Where(b => versionSpec.Satisfies(b.Version));
			var filteredPackage = new Package().PopulateWithNonDefaultValues(package);
			filteredPackage.Builds = matchingBuilds.ToArray();
			return filteredPackage;
		}

		public async Task<Package> FetchPackage(string packageName) {
			var req = WebRequest.CreateHttp(new Uri(baseUri, $"/{packageName}"));
			var reqStream = await req.GetRequestStreamAsync();
			var res = await reqStream.ReadToEndAsync();
			var package = JsonConvert.DeserializeObject<Package>(Encoding.UTF8.GetString(res));
			return package;
		}

		public async Task<IEnumerable<Package>> FetchPackageList() {
			var req = WebRequest.CreateHttp(new Uri(baseUri, "/"));
			var reqStream = await req.GetRequestStreamAsync();
			var res = await reqStream.ReadToEndAsync();
			var packageList = JsonConvert.DeserializeObject<IEnumerable<Package>>(Encoding.UTF8.GetString(res));
			return packageList;
		}

		public async Task<IEnumerable<Package>> FetchPackageList(DateTime updatedAfter) {
			var updatedAfterTimestamp = (long)(updatedAfter.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			var req = WebRequest.CreateHttp(new Uri(baseUri, $"/?LastSync={updatedAfterTimestamp}"));
			var reqStream = await req.GetRequestStreamAsync();
			var res = await reqStream.ReadToEndAsync();
			var packageList = JsonConvert.DeserializeObject<IEnumerable<Package>>(Encoding.UTF8.GetString(res));
			return packageList;
		}
	}
}
