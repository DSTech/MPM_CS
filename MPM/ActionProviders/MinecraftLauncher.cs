using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autofac;
using MPM.Core;
using MPM.Core.Instances;
using MPM.Core.Instances.Cache;
using MPM.Core.Profiles;
using MPM.Extensions;
using MPM.Net.Protocols.Minecraft;
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
            var packageNames = instance.Configuration.Packages.Select(p => p.PackageName).ToArray();
            var hasMinecraft = packageNames.Contains("minecraft");
            var hasForge = packageNames.Contains("minecraftforge");
            if (!hasMinecraft) {
                throw new Exception("This launcher cannot launch this instance, as it does not contain Minecraft");
            }

            var cacheManager = resolver.Resolve<GlobalStorage>().FetchGlobalCache();
            var installedMinecraftBuild = instance.Configuration.Packages.First(b => b.PackageName == "minecraft");
            var mdc = new MinecraftDownloadClient();

            //TODO: Load this from cacheManager, or cache it if it is missing
            var versionDetails = mdc.FetchVersion(installedMinecraftBuild.Arch);
            //var assetIndex = mdc.FetchAssetIndex(versionDetails.AssetIndex);

            var launchArgs = versionDetails.FillArguments(
                profile.Name,
                versionDetails.Id,
                instance.Location.FullName,
                instance.Location.CreateSubdirectory("assets").FullName,
                versionDetails.AssetIndex.Id,
                profile.YggdrasilProfileId,
                profile.YggdrasilAccessToken,
                profile.YggdrasilUserType,
                new Dictionary<string, string>()// profile.CreateUserProperties()
            );

            Console.WriteLine("Default Minecraft launch arguments are:\n\t{0}", versionDetails.MinecraftArguments);
            Console.WriteLine("Launch arguments are:\n\t{0}", launchArgs);

            //throw new NotImplementedException();
        }

        protected virtual void Dispose(bool Disposing) {
            if (!Disposing) {
                return;
            }
            //Dispose of resources here
        }
    }
}
