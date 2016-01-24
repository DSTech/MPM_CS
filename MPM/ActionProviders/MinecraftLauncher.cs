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
