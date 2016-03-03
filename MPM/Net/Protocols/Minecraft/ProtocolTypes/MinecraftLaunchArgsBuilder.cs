using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace MPM.Net.Protocols.Minecraft.ProtocolTypes {
    class MinecraftLaunchArgsBuilder {
        public string Java { get; set; } = "";
        public string UserName { get; set; } = "";
        public string VersionId { get; set; } = "";
        public DirectoryInfo InstanceDirectory { get; set; }
        public DirectoryInfo AssetsDirectory { get; set; }
        public string AssetsIndexId { get; set; } = "";
        public string AuthProfileId { get; set; } = "";
        public string AuthAccessToken { get; set; } = "";
        public string AuthUserType { get; set; } = "";
        public Dictionary<string, string> UserProperties { get; set; } = new Dictionary<string, string>();
        public List<string> ClassPaths { get; set; } = new List<string>();
        public string NativesPath { get; set; } = "";
        public string LaunchClass { get; set; } = "";
        public string TweakClass { get; set; } = "";
        public int XmsMb { get; set; } = 256;
        public int XmxMb { get; set; } = 2048;

        private static string GetBestJavaPath(string specifiedJava) {
            if (!string.IsNullOrWhiteSpace(specifiedJava)) {
                return specifiedJava;
            }
            return Path.Combine(GetJavaInstallationPath(), "bin", "java.exe");
        }

        private static string GetJavaInstallationPath() {
            var environmentPath = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(environmentPath)) {
                return environmentPath;
            }
            var toDispose = new List<IDisposable>();
            string javaRegistryKey;
            RegistryKey rk;
            if (Environment.Is64BitOperatingSystem) {//Prefer 64bit if it is an option
                var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                javaRegistryKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
                rk = baseKey.OpenSubKey(javaRegistryKey);
                if(rk != null) {
                    toDispose.Add(baseKey);
                } else {
                    baseKey.Dispose();
                }
                //if rk is null, it will default to trying 32-bit
            } else {
                rk = null;
            }
            if (rk == null) {
                var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
                javaRegistryKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
                rk = baseKey.OpenSubKey(javaRegistryKey);
                if (rk != null) {
                    toDispose.Add(baseKey);
                } else {
                    baseKey.Dispose();
                }
            }
            if (rk == null) {
                foreach (var disp in toDispose) {
                    disp.Dispose();
                }
                throw new Exception("Java not found!");
            }
            string retval;
            using (rk) {
                var currentVersion = rk.GetValue("CurrentVersion")?.ToString();
                if (currentVersion == null) {
                    var lastKey = rk.GetSubKeyNames().LastOrDefault();
                    if (lastKey == null) {
                        foreach (var disp in toDispose) {
                            disp.Dispose();
                        }
                        throw new Exception("Java not found!");
                    }
                    using (var key = rk.OpenSubKey(lastKey)) {
                        retval = key.GetValue("JavaHome").ToString();
                    }
                } else {
                    using (var key = rk.OpenSubKey(currentVersion)) {
                        retval = key.GetValue("JavaHome").ToString();
                    }
                }
            }
            foreach (var disp in toDispose) {
                disp.Dispose();
            }
            return retval;
        }

        public string Build(string minecraftArgumentsTemplate) {

            var minecraftArgs = minecraftArgumentsTemplate
                .Replace("${auth_player_name}", $"\"{this.UserName}\"")
                .Replace("${version_name}", $"\"{this.VersionId}\"")
                .Replace("${game_directory}", $"\"{this.InstanceDirectory.FullName}\"")
                .Replace("${assets_root}", $"\"{this.AssetsDirectory.FullName}\"")
                .Replace("${assets_index_name}", $"\"{this.AssetsIndexId}\"")
                .Replace("${auth_uuid}", $"\"{this.AuthProfileId}\"")
                .Replace("${auth_access_token}", $"\"{this.AuthAccessToken}\"")
                .Replace("${user_type}", $"\"{this.AuthUserType}\"")
                .Replace("${user_properties}", JsonConvert.SerializeObject(this.UserProperties));


            var Xms = $"-Xms{XmsMb}M";

            var Xmx = $"-Xmx{XmxMb}M";

            var userHome = $"-Duser.home=\"{InstanceDirectory}\"";

            var nativesPaths = $"-Djava.library.path=\"{NativesPath}\" -Dorg.lwjgl.librarypath=\"{NativesPath}\" -Dnet.java.games.input.librarypath=\"{NativesPath}\"";

            var classPath = $"-cp {string.Join(";", ClassPaths.Select(p => $"\"{p}\""))}";

            var heapDumpPath = "-XX:HeapDumpPath=MojangTricksIntelDriversForPerformance_javaw.exe_minecraft.exe.heapdump";

            var preferIPv4 = "-Djava.net.preferIPv4Stack=true";

            return $"{Xms} {Xmx} {nativesPaths} {userHome} {heapDumpPath} {preferIPv4} {classPath} {LaunchClass} {minecraftArgs} {TweakClass}";
        }

        public string GetJava() {
            var javaPath = GetBestJavaPath(Java);
            return javaPath;
        }
    }
}
