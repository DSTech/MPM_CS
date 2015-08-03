using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Net.Protocols.Minecraft.DTO;
using Newtonsoft.Json;
using NServiceKit.Service;
using NServiceKit.ServiceClient;
using NServiceKit.ServiceClient.Web;
using RestSharp;
using semver.tools;

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
		private readonly RestClient client;

		public MinecraftDownloadClient() {
			this.client = new RestClient("https://s3.amazonaws.com/Minecraft.Download/");
		}

		public async Task<MinecraftVersionCollection> FetchVersions() {
			var req = new RestRequest("/versions/versions.json") {
			};
			var res = await client.ExecuteGetTaskAsync(req);
			var data = JsonConvert.DeserializeObject<MinecraftVersionCollection>(res.Content);
			return data;
		}

		public async Task<MinecraftVersion> FetchVersion(string versionId) {
			var req = new RestRequest(String.Format("/versions/{0}/{0}.json", versionId)) {
				OnBeforeDeserialization = resp => resp.ContentType = "application/json",
			};
			var res = await client.ExecuteGetTaskAsync<MinecraftVersion>(req);
			return res.StatusCode == System.Net.HttpStatusCode.OK ? res.Data : null;
		}

		public async Task<MinecraftVersion> FetchLatest(bool snapshot = false) {
			var versionList = await FetchVersions();
			return await FetchVersion(snapshot ? versionList.Latest.Snapshot : versionList.Latest.Release);
		}
	}
}
