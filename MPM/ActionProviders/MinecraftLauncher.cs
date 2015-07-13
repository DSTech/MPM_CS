using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MPM.Core;
using MPM.Core.Instances;
using MPM.Core.Profiles;

namespace MPM {
	public class MinecraftLauncher : ILauncher, IDisposable {
		public string UserName { get; set; }
		public void Launch(Instance instance, IProfile profile) {
			throw new NotImplementedException();
		}

		public void Dispose() {
			Dispose(true);
		}

		protected virtual void Dispose(bool Disposing) {
			if (!Disposing) {
				return;
			}
			//Dispose of resources here
		}
	}
}
