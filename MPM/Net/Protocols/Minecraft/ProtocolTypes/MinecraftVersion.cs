using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;
using MPM.Util.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    public class MinecraftVersion {
        public MinecraftVersion() {
        }

        public MinecraftVersion(
            string id,
            DateTime releaseTime,
            ReleaseType releaseType,
            string minecraftArguments,
            int minimumLauncherVersion,
            string assetsIdentifier,
            IEnumerable<LibrarySpec> libraries
            ) {
            this.Id = id;
            this.ReleaseTime = releaseTime;
            this.Type = releaseType;
            this.MinecraftArguments = minecraftArguments;
            this.MinimumLauncherVersion = minimumLauncherVersion;
            this.AssetsIdentifier = assetsIdentifier;
            this.Libraries = libraries.Denull().ToList();
        }

        [JsonProperty("assetIndex")]
        public AssetIndexSpecifier @AssetIndex { get; set; }

        /// <summary>
        ///     Version for which assets should be loaded, eg: "1.7.10"
        /// </summary>
        [JsonProperty("assets")]
        public string AssetsIdentifier { get; set; }

        [JsonProperty("downloads")]
        public MinecraftDownloads Downloads { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("libraries")]
        public List<LibrarySpec> Libraries { get; set; }

        [JsonProperty("mainClass")]
        public string MainClass { get; set; }

        /// <summary>
        ///     A string with ${placeholders} that should be substituted before launching, using the string as command line
        ///     arguments
        /// </summary>
        [JsonProperty("minecraftArguments")]
        public string MinecraftArguments { get; set; }

        /// <summary>
        ///     Minimum launcher spec version capable of launching the particular version
        /// </summary>
        [JsonProperty("minimumLauncherVersion")]
        public int MinimumLauncherVersion { get; set; }

        /// <summary>
        ///     Time of release in string format, eg: 2014-05-14T19:29:23+02:00
        /// </summary>
        [JsonProperty("releaseTime")]
        private DateTimeOffset _releaseTime;

        [JsonIgnore]
        public DateTime ReleaseTime {
            get { return _releaseTime.UtcDateTime; }
            set { _releaseTime = new DateTimeOffset(value, TimeSpan.Zero); }
        }

        [JsonProperty("time")]
        private DateTimeOffset _time;

        [JsonIgnore]
        public DateTime Time {
            get { return _time.UtcDateTime; }
            set { _time = new DateTimeOffset(value, TimeSpan.Zero); }
        }

        /// <summary>
        ///     <see cref="ReleaseType" /> of version, eg <see cref="ReleaseType.Release" /> or <see cref="ReleaseType.Snapshot" />
        /// </summary>
        [JsonProperty("type"), JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ReleaseType Type { get; set; }
    }
}
