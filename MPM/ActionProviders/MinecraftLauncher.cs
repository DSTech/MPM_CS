using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Autofac;
using MPM.Core;
using MPM.Core.Instances;
using MPM.Core.Instances.Cache;
using MPM.Core.Profiles;
using MPM.Extensions;
using MPM.Net.Protocols.Minecraft;
using MPM.Net.Protocols.Minecraft.ProtocolTypes;
using MPM.Types;
using Newtonsoft.Json;

namespace MPM.ActionProviders {
    public class MinecraftLauncher : ILauncher, IDisposable {
        public string UserName { get; set; }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Launch(IContainer resolver, Instance instance, IProfile _profile) {
            MutableProfile profile = _profile.ToMutableProfile();
            Console.WriteLine("Launching instance {0} with profile {1}...", instance.Location.FullName, profile.Name);
            var packagesByName = instance.Configuration.Packages.ToDictionary(p => p.PackageName);
            var hasMinecraft = packagesByName.ContainsKey("minecraft");
            var hasForge = packagesByName.ContainsKey("minecraftforge");
            if (!hasMinecraft) {
                throw new Exception("This launcher cannot launch this instance, as it does not contain Minecraft");
            }

            var cacheManager = resolver.Resolve<GlobalStorage>().FetchGlobalCache();
            var installedMinecraftBuild = packagesByName["minecraft"];
            var mdc = new MinecraftDownloadClient();
            
            var versionDetails = GetAndEnsureVersionDetailsCached(installedMinecraftBuild.Arch, mdc, cacheManager);

            var launchArgsBuilder = new MinecraftLaunchArgsBuilder() {
                UserName = profile.Name,
                VersionId = versionDetails.Id,
                InstanceDirectory = instance.Location,
                AssetsDirectory = instance.Location.CreateSubdirectory("assets"),
                AssetsIndexId = versionDetails.AssetIndex.Id,
                AuthProfileId = profile.YggdrasilProfileId,
                AuthAccessToken = profile.YggdrasilAccessToken,
                AuthUserType = profile.YggdrasilUserType,
                UserProperties = new Dictionary<string, string>(),//TODO: Support custom user properties
            };

            if (instance.Side == CompatibilitySide.Client) {
                //Create classpath
                var classPathFiles = new List<FileInfo>();

                var clientJarFile = instance.Location.SubFile("client.jar");
                classPathFiles.Add(clientJarFile);

                //var installingPlatform = Environment.OSVersion.Platform;
                //var installingBitness64 = Environment.Is64BitOperatingSystem;
                //var nonNativeLibs = versionDetails.Libraries.Where(l => l.AppliesToPlatform(installingPlatform)).Where(l => l.ApplyNatives(installingPlatform, installingBitness64).Artifact == null);
                //TODO: Install from non-native libs, accounting for extract-based non-natives if they are actually something that occurs

                foreach (var lib in instance.Location.CreateSubdirectory("libraries")
                    .EnumerateFiles("*.jar", SearchOption.AllDirectories)
                    .Where(f => f.Extension == ".jar")) {
                    classPathFiles.Add(lib);
                }

                launchArgsBuilder.ClassPaths = classPathFiles.Select(cpf => cpf.FullName).ToList();

                launchArgsBuilder.NativesPath = instance.Location.CreateSubdirectory("bin").CreateSubdirectory("natives").FullName;

                launchArgsBuilder.LaunchClass = versionDetails.MainClass;
            } else {
                //TODO: Serverside components
            }

            var java = launchArgsBuilder.GetJava();
            var launchArgs = launchArgsBuilder.Build(versionDetails.MinecraftArguments);

            Console.WriteLine("Default Minecraft launch arguments are:\n\t{0}", versionDetails.MinecraftArguments);
            Console.WriteLine("Launch arguments are:\n\t{0}", launchArgs);
            {
                var startInfo = new ProcessStartInfo(java, launchArgs);
                startInfo.UseShellExecute = false;
                var proc = Process.Start(startInfo);
                proc.WaitForExit();
            }
        }

        private static MinecraftVersion GetAndEnsureVersionDetailsCached(SemVersion arch, MinecraftDownloadClient mdc, ICacheManager cacheManager) {
            var cacheId = cacheManager.NamingProvider.GetNameForArchVersionDetails("minecraft", arch);
            MinecraftVersion versionDetails;
            if (!cacheManager.Contains(cacheId)) {
                versionDetails = mdc.FetchVersion(arch);
                cacheManager.StoreAsJson(cacheId, versionDetails);
            } else {
                versionDetails = cacheManager.FetchFromJson<MinecraftVersion>(cacheId);
            }
            return versionDetails;
        }

        protected virtual void Dispose(bool Disposing) {
            if (!Disposing) {
                return;
            }
            //Dispose of resources here
        }
    }
}
