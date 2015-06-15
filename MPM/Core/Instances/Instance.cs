using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Dependency;
using MPM.Core.FileSystem;
using MPM.Core.Instances.Installation;

namespace MPM.Core.Instances {
	public class Instance {
		public String Name { get; set; }
        public Uri Location { get; set; }
		public Type LauncherType { get; set; } = typeof(MinecraftLauncher);//TODO: Change to a default (ScriptLauncher / ShellLauncher?), or auto-identify launch method

		public IFileMap GetFileMap() {
			return this.Configuration.GenerateFileMap();
		}

		public IFileSystem GetFileSystem() {
			return new LocalFileSystem(Location);
		}

		public InstanceConfiguration @Configuration {
			get {
				throw new NotImplementedException();
			}
		}

		public ILauncher CreateLauncher() {
			var ctor = LauncherType.GetConstructor(new Type[0]);
			return (ILauncher)ctor.Invoke(new object[0]);
		}
	}
}
