using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using MPM.Extensions;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    class MinecraftLaunchArgsBuilder {
        public string UserName { get; set; } = "";
        public string VersionId { get; set; } = "";
        public DirectoryInfo InstanceDirectory { get; set; }
        public DirectoryInfo AssetsDirectory { get; set; }
        public string AssetsIndexId { get; set; } = "";
        public string AuthProfileId { get; set; } = "";
        public string AuthAccessToken { get; set; } = "";
        public string AuthUserType { get; set; } = "";
        public Dictionary<string, string> UserProperties { get; set; } = new Dictionary<string, string>();
        public string TweakClass { get; set; } = "";

        public IEnumerable<string> Build(string minecraftArgumentsTemplate) {

            var minecraftArgs = minecraftArgumentsTemplate
                .Replace("${auth_player_name}", $"\"{this.UserName}\"")
                .Replace("${version_name}", $"\"{this.VersionId}\"")
                .Replace("${game_directory}", $"\"{this.InstanceDirectory.FullName}\"")
                .Replace("${assets_root}", $"\"{this.AssetsDirectory.FullName}\"")
                .Replace("${assets_index_name}", $"\"{this.AssetsIndexId}\"")
                .Replace("${auth_uuid}", $"\"{this.AuthProfileId}\"")
                .Replace("${auth_access_token}", $"\"{this.AuthAccessToken}\"")
                .Replace("${user_type}", $"\"{this.AuthUserType}\"")
                .Replace("${user_properties}", JsonConvert.SerializeObject(this.UserProperties ?? new Dictionary<string, string>()));

            var tweakClass = TweakClass.IsNullOrWhiteSpace() ? new string[0] : new string[] { "--tweakClass", this.TweakClass.ToString() };

            return new [] { minecraftArgs }.Concat(tweakClass);
        }
    }
}
