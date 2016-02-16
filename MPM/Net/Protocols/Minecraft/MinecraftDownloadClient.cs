using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using MPM.Extensions;
using MPM.Net.Protocols.Minecraft.ProtocolTypes;
using MPM.Types;
using Newtonsoft.Json;
using RestSharp;

namespace MPM.Net.Protocols.Minecraft {
    public class MinecraftDownloadClient {
        private readonly RestClient client;

        public MinecraftDownloadClient() {
            this.client = new RestClient("https://s3.amazonaws.com/Minecraft.Download/") {
                CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.CacheIfAvailable),
            };
        }

        public MinecraftVersionCollection FetchVersions() {
            var req = new RestRequest("/versions/versions.json");
            var res = client.ExecuteAsGet(req, "GET");
            var data = JsonConvert.DeserializeObject<MinecraftVersionCollection>(res.Content);
            return data;
        }

        public MinecraftVersion FetchVersion(string versionId) {
            var req = new RestRequest(String.Format("/versions/{0}/{0}.json", versionId));
            var res = client.ExecuteAsGet<MinecraftVersion>(req, "GET");
            var data = JsonConvert.DeserializeObject<MinecraftVersion>(res.Content);
            return data;
        }

        public async Task<Stream> FetchArtifact(ArtifactSpec artifactSpec) {
            var url = artifactSpec.Url;
            var req = WebRequest.Create(url);
            var res = await req.GetResponseAsync();
            return res.GetResponseStream().AndDispose(res);
        }

    }
}
