using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPM.Core;
using MPM.Core.Instances;
using MPM.Core.Profiles;

namespace MPM.ActionProviders {
    public class MinecraftLauncher : ILauncher, IDisposable {
        public string UserName { get; set; }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Launch(Instance instance, IProfile profile) {
            Console.WriteLine("Launching instance {0} with profile {1}...", instance.Location, profile.Name);
            var packageNames = instance.Configuration.Packages.Select(p => p.PackageName).ToArray();
            var hasMinecraft = packageNames.Contains("minecraft");
            var hasForge = packageNames.Contains("minecraftforge");
            if (!hasMinecraft) {
                throw new Exception("This launcher cannot launch this instance, as it does not contain Minecraft");
            }
            throw new NotImplementedException();
        }

        protected virtual void Dispose(bool Disposing) {
            if (!Disposing) {
                return;
            }
            //Dispose of resources here
        }
    }
}
