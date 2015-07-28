using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MPM.Net;
using MPM.Types;
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

		public async Task<Build> FetchBuild(string packageName, SemanticVersion version, CompatibilitySide side, Arch arch, CompatibilityPlatform platform) {
			//TODO: Support "arch" and "platform" filters?
			var req = WebRequest.CreateHttp(new Uri(baseUri, $"/packages/{packageName}/{version}"));
			var response = await req.GetResponseAsync();
			byte[] responseData;
			using (var responseStream = response.GetResponseStream()) {
				responseData = await responseStream.ReadToEndAsync();
			}
			var build = JsonConvert.DeserializeObject<MPM.Net.DTO.Build>(Encoding.UTF8.GetString(responseData));
			build.Package = build.Package ?? packageName;
			return build.FromDTO();
		}

		public async Task<IEnumerable<Build>> FetchBuilds(string packageName, VersionSpec versionSpec) {
			var package = await FetchPackage(packageName);
			var matchingBuilds = package
				.Builds
				.Where(b => versionSpec.Satisfies(b.Version))
				.ToArray();
			return matchingBuilds;
		}

		public async Task<Package> FetchPackage(string packageName) {
			var req = WebRequest.CreateHttp(new Uri(baseUri, $"/packages/{packageName}"));
			var response = await req.GetResponseAsync();
			byte[] responseData;
			using (var responseStream = response.GetResponseStream()) {
				responseData = await responseStream.ReadToEndAsync();
			}
			var package = JsonConvert.DeserializeObject<MPM.Net.DTO.Package>(Encoding.UTF8.GetString(responseData));
			return package.FromDTO();
		}

		public async Task<IEnumerable<Package>> FetchPackageList() {
			var req = WebRequest.CreateHttp(new Uri(baseUri, "/packages/"));
			var response = await req.GetResponseAsync();
			byte[] responseData;
			using (var responseStream = response.GetResponseStream()) {
				responseData = await responseStream.ReadToEndAsync();
			}
			var packageList = JsonConvert.DeserializeObject<IEnumerable<MPM.Net.DTO.Package>>(Encoding.UTF8.GetString(responseData));
			return packageList.Select(package => package.FromDTO()).ToArray();
		}

		public async Task<IEnumerable<Package>> FetchPackageList(DateTime updatedAfter) {
			var updatedAfterTimestamp = (long)(updatedAfter.ToUniversalTime().Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
			var req = WebRequest.CreateHttp(new Uri(baseUri, $"/packages/?LastSync={updatedAfterTimestamp}"));
			var response = await req.GetResponseAsync();
			byte[] responseData;
			using (var responseStream = response.GetResponseStream()) {
				responseData = await responseStream.ReadToEndAsync();
			}
			var packageList = JsonConvert.DeserializeObject<IEnumerable<MPM.Net.DTO.Package>>(Encoding.UTF8.GetString(responseData));
			return packageList.Select(package => package.FromDTO()).ToArray();
		}
	}
}
