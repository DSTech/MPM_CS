using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MPM.Net.Protocols.Minecraft.ProtocolTypes;
using MPM.Types;
using MPM.Util;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft {
    public class MinecraftDownloadClient {
        public MinecraftDownloadClient() {
        }

        public readonly string BaseUrl = "https://s3.amazonaws.com/Minecraft.Download";

        private System.Net.Cache.RequestCachePolicy _cachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.CacheIfAvailable);

        private T GetFromUrlAsType<T>(string url) {
            var req = WebRequest.Create(url);
            req.CachePolicy = _cachePolicy;
            var res = req.GetResponse();
            var rawData = Encoding.UTF8.GetString(res.GetResponseStream().ReadToEndAndClose());
            var data = JsonConvert.DeserializeObject<T>(rawData);
            return data;
        }

        public MinecraftVersionCollection FetchVersions() => this.GetFromUrlAsType<MinecraftVersionCollection>($"{this.BaseUrl}/versions/versions.json");

        public MinecraftVersion FetchVersion(SemVersion versionId) => this.GetFromUrlAsType<MinecraftVersion>(String.Format("{0}/versions/{1}/{1}.json", this.BaseUrl, versionId));

        public AssetIndex FetchAssetIndex(MinecraftVersion details) => this.GetFromUrlAsType<AssetIndex>(details.AssetIndex.Url);

        public async Task<Stream> FetchClient(MinecraftVersion version) {
            var url = version.Downloads.Client.Url;
            var req = WebRequest.Create(url);
            var res = await req.GetResponseAsync();
            return res.GetResponseStream().AndDispose(res);
        }

        public async Task<Stream> FetchServer(MinecraftVersion version) {
            var url = version.Downloads.Server.Url;
            var req = WebRequest.Create(url);
            var res = await req.GetResponseAsync();
            return res.GetResponseStream().AndDispose(res);
        }

        private static async Task<Stream> FetchUrlAsStreamAsync(string assetUrl) {
            var req = WebRequest.Create(assetUrl);
            var res = await req.GetResponseAsync();
            return res.GetResponseStream().AndDispose(res);
        }

        public Task<Stream> FetchArtifact(ArtifactSpec artifactSpec) {
            return FetchUrlAsStreamAsync(artifactSpec.Url);
        }

        ///<summary>
        /// Assets download from http://resources.download.minecraft.net/<first-2-hex-letters-of-hash>/<whole-hash>
        /// and should be saved at .minecraft/assets/objects/<first-2-hex-letters-of-hash>/<whole-hash>
        /// with a copy stored in .minecraft/assets/virtual/legacy/ for 1.7.2 and below
        ///</summary>
        public Task<Stream> FetchAsset(Asset asset) {
            var assetHex = Hex.GetString(asset.Hash.Checksum).ToLowerInvariant();
            var assetSubHash = assetHex.Substring(0, 2);
            var assetUrl = $"http://resources.download.minecraft.net/{assetSubHash}/{assetHex}";
            return FetchUrlAsStreamAsync(assetUrl);
        }
    }
}
