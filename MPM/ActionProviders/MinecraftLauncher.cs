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

        public void Launch(IContainer resolver, Instance instance, IProfile profile) {
            MutableProfile mutableProfile;

            if (instance.Side == CompatibilitySide.Client) {
                if (profile == null) {
                    throw new ArgumentNullException(nameof(profile), "This launcher requires a profile to launch client instances, and none was provided");
                }
                mutableProfile = profile.ToMutableProfile();
                Debug.Assert(mutableProfile != null);
                Console.WriteLine("Launching instance {0} with profile {1}...", instance.Location.FullName, mutableProfile.Name);
            } else {
                mutableProfile = null;//Server launcher does not require a profile
            }

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

            var javaLauncher = new Util.JavaLauncher(prefer64Bit: true);

            var launchArgs = new List<string>();

            if (instance.Side == CompatibilitySide.Client) {
                //Client launching
                Debug.Assert(mutableProfile != null);
                var launchArgsBuilder = new MinecraftLaunchArgsBuilder() {
                    UserName = mutableProfile.Name,
                    VersionId = versionDetails.Id,
                    InstanceDirectory = instance.Location,
                    AssetsDirectory = instance.Location.CreateSubdirectory("assets"),
                    AssetsIndexId = versionDetails.Id,
                    AuthProfileId = mutableProfile.YggdrasilProfileId,
                    AuthAccessToken = mutableProfile.YggdrasilAccessToken,
                    AuthUserType = mutableProfile.YggdrasilUserType,
                    UserProperties = new Dictionary<string, string>(),//TODO: Support custom user properties
                };

                {
                    //Add jar file to classpaths
                    var clientJarFile = instance.Location.SubFile("client.jar");
                    javaLauncher.ClassPaths.Add(clientJarFile);
                }

                {
                    //Add classpaths for libraries
                    //TODO: Install from non-native libs, accounting for extract-based non-natives if they are actually something that occurs (Update: They are not[Citation Needed])
                    //var installingPlatform = Environment.OSVersion.Platform;
                    //var installingBitness64 = Environment.Is64BitOperatingSystem;
                    //var nonNativeLibs = versionDetails.Libraries.Where(l => l.AppliesToPlatform(installingPlatform)).Where(l => l.ApplyNatives(installingPlatform, installingBitness64).Artifact == null);

                    //Replace this with the above behaviour
                    var libraryFiles = instance.Location.CreateSubdirectory("libraries")
                        .EnumerateFiles("*.jar", SearchOption.AllDirectories)
                        .Where(f => f.Extension == ".jar");
                    javaLauncher.ClassPaths.AddRange(libraryFiles);
                }

                javaLauncher.NativesPaths.Add(instance.Location.CreateSubdirectory("bin").CreateSubdirectory("natives"));

                javaLauncher.LaunchClass = versionDetails.MainClass;

                launchArgs.AddRange(launchArgsBuilder.Build(versionDetails.MinecraftArguments));
            } else {
                //Server launching
                {
                    //Add jar file to classpaths
                    var serverJarFile = instance.Location.SubFile("server.jar");
                    javaLauncher.ClassPaths.Add(serverJarFile);
                }

                javaLauncher.LaunchJar = instance.Location.SubFile("server.jar");

                javaLauncher.AdditionalArguments.Add("nogui");
            }

            {
                if (instance.Side == CompatibilitySide.Server) {
                    var eulaFile = instance.Location.SubFile("eula.txt");
                    if (!eulaFile.Exists) {
                        Console.Write("Do you accept the Minecraft EULA at \"https://account.mojang.com/documents/minecraft_eula\"?\n[Y/N]> ");
                        var response = Console.ReadLine();
                        if (response?.ToLowerInvariant() != "y") {
                            throw new ApplicationException("User did not accept Minecraft server EULA");
                        }
                        using (var eulaWriter = new StreamWriter(eulaFile.OpenWrite())) {
                            eulaWriter.WriteLine("eula=true");
                        }
                    }
                }
            }
            using (var proc = javaLauncher.Launch(launchArgs)) {
                Console.WriteLine();
                Console.WriteLine(proc.StartInfo.Arguments);
                Console.WriteLine();
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
