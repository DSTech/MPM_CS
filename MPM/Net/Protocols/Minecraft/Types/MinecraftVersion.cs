using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;

namespace MPM.Net.Protocols.Minecraft.Types {
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

        public string Id { get; set; }

        /// <summary>
        ///     Time of release in string format, eg: 2014-05-14T19:29:23+02:00
        /// </summary>
        public DateTime ReleaseTime { get; set; }

        /// <summary>
        ///     <see cref="ReleaseType" /> of version, eg <see cref="ReleaseType.Release" /> or <see cref="ReleaseType.Snapshot" />
        /// </summary>
        public ReleaseType Type { get; set; }

        /// <summary>
        ///     A string with ${placeholders} that should be substituted before launching, using the string as command line
        ///     arguments
        /// </summary>
        public string MinecraftArguments { get; set; }

        /// <summary>
        ///     Minimum launcher spec version capable of launching the particular version
        /// </summary>
        public int MinimumLauncherVersion { get; set; }

        /// <summary>
        ///     Version for which assets should be loaded, eg: "1.7.10"
        /// </summary>
        public string AssetsIdentifier { get; set; }

        public List<LibrarySpec> Libraries { get; set; }

        public string FillArguments(
            string username,
            string versionName,
            string gameDir,
            string assetsDir,
            string assetsIndex,
            string uuid,
            string accessToken,
            string userType
            ) {
            return MinecraftArguments
                .Replace("${auth_player_name}", $"\"{username}\"")
                .Replace("${version_name}", $"\"{versionName}\"")
                .Replace("${game_directory}", $"\"{gameDir}\"")
                .Replace("${assets_root}", $"\"{assetsDir}\"")
                .Replace("${assets_index_name}", $"\"{assetsIndex}\"")
                .Replace("${auth_uuid}", $"\"{uuid}\"")
                .Replace("${auth_access_token}", $"\"{accessToken}\"")
                .Replace("${user_type}", $"\"{userType}\"")
                ;
        }
    }
}
