using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MPM {
	public class MinecraftLauncher : IDisposable {
		public string UserName { get; set; }
		public void Launch() {
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
