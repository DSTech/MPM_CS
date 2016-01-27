using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;
using MPM.Net.Protocols.Minecraft.Types;
using Newtonsoft.Json;
using RestSharp;

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
            var req = new RestRequest("/versions/versions.json");
            var res = await client.ExecuteGetTaskAsync(req);
            var data = JsonConvert.DeserializeObject<DTO.MinecraftVersionCollection>(res.Content);
            return data.FromDTO();
        }

        public async Task<MinecraftVersion> FetchVersion(string versionId) {
            var req = new RestRequest(String.Format("/versions/{0}/{0}.json", versionId));
            var res = await client.ExecuteGetTaskAsync<DTO.MinecraftVersion>(req);
            var data = JsonConvert.DeserializeObject<DTO.MinecraftVersion>(res.Content);
            return data.FromDTO();
        }

        public async Task<MinecraftVersion> FetchLatest(bool snapshot = false) {
            var versionList = await FetchVersions();
            return await FetchVersion(snapshot ? versionList.LatestSnapshot.Id : versionList.LatestRelease.Id);
        }

        /// <summary>
        /// </summary>
        /// <param name="package"></param>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <param name="nativeVariant">'${arch}' tag should already be replaced within this string with '32' or '64'</param>
        /// <returns></returns>
        public async Task<Stream> FetchLibrary(string package, string name, string version, string nativeVariant = null) {
            const string baseUrl = "https://libraries.minecraft.net";
            string url;
            if (!String.IsNullOrWhiteSpace(nativeVariant)) {
                url = $"{baseUrl}/{package}/{name}/{version}/{name}-{version}-{nativeVariant}.jar";
            } else {
                url = $"{baseUrl}/{package}/{name}/{version}/{name}-{version}.jar";
            }
            var req = WebRequest.Create(url);
            var res = await req.GetResponseAsync();
            return res.GetResponseStream().AndDispose(res);
        }
    }
}
