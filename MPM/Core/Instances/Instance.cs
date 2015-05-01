using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.FileSystem;

namespace MPM.Core.Instances {
	public class Instance {
		public String Name { get; set; }
        public Uri Location { get; set; }
		public Type LauncherType { get; set; } = typeof(MinecraftLauncher);//TODO: Change to a default (ScriptLauncher / ShellLauncher?), or auto-identify launch method

		public IFileIndex GetFileIndex() {
			throw new NotImplementedException();
		}

		public IFileSystem GetFileSystem() {
			throw new NotImplementedException();
		}

		public ILauncher CreateLauncher() {
			var ctor = LauncherType.GetConstructor(new Type[0]);
			return ctor.Invoke(new object[0]) as ILauncher;
		}
	}
}
