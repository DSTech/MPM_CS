using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Net.Protocols.Minecraft.DTO;
using NServiceKit.Service;
using NServiceKit.ServiceClient;
using NServiceKit.ServiceClient.Web;

namespace MPM.Net.Protocols.Minecraft {

	public static class MinecraftDownloadClientExtensions {

		public static Task<MinecraftVersion> FetchLatestRelease(this MinecraftDownloadClient minecraftDownloadClient) {
			return minecraftDownloadClient.FetchLatest(snapshot: false);
		}

		public static Task<MinecraftVersion> FetchLatestSnapshot(this MinecraftDownloadClient minecraftDownloadClient) {
			return minecraftDownloadClient.FetchLatest(snapshot: true);
		}
	}

	public class MinecraftDownloadClient {
		public readonly IServiceClient serviceClient;

		public MinecraftDownloadClient() {
			serviceClient = new JsonServiceClient();
		}

		public async Task<MinecraftVersionCollection> FetchVersions() {
			return await serviceClient.GetAsync<MinecraftVersionCollection>("https://s3.amazonaws.com/Minecraft.Download/versions/versions.json");
		}

		public async Task<MinecraftVersion> FetchVersion(string versionId) {
			return await serviceClient.GetAsync<MinecraftVersion>(String.Format("https://s3.amazonaws.com/Minecraft.Download/versions/{0}/{0}.json", versionId));
		}

		public async Task<MinecraftVersion> FetchLatest(bool snapshot = false) {
			var versionList = await FetchVersions();
			return await FetchVersion(snapshot ? versionList.Latest.Snapshot : versionList.Latest.Release);
		}
	}
}
