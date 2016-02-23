using System;
using System.Collections.Generic;
using System.Linq;
using MPM.Types;
using MPM.Util.Json;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class AssetIndex {
        [JsonObject]
        private struct PseudoAsset {
            [JsonProperty("hash", Required = Required.Always, Order = 1)]
            [JsonConverter(typeof(Sha1HashHexConverter))]
            public Hash @Hash { get; set; }

            [JsonProperty("size", Required = Required.Always, Order = 2)]
            public long Size { get; set; }
        }

        [JsonProperty("objects", Required = Required.Always)]
        private Dictionary<Uri, PseudoAsset> _assets {
            get {
                if (Assets == null) {
                    return null;
                }
                return Assets.ToDictionary<Asset, Uri, PseudoAsset>((Asset asset) => asset.Uri, (Asset asset) => new PseudoAsset {
                    Hash = asset.Hash,
                    Size = asset.Size,
                });
            }
            set {
                if (value == null) {
                    Assets = null;
                }
                Assets = new List<Asset>(
                    from item in value
                    select new Asset {
                        Uri = item.Key,
                        Hash = item.Value.Hash,
                        Size = item.Value.Size,
                    });
            }
        }

        [JsonIgnore]
        public List<Asset> Assets { get; set; }
    }
}
