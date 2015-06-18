using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Dependency;
using MPM.Core.Instances.Installation;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Platform.VirtualFileSystem;
using Platform.VirtualFileSystem.Providers.Local;

namespace MPM.Core.Instances {
	public class Instance {
		public String Name { get; set; }
		public String Location { get; set; }
		public Type LauncherType { get; set; } = typeof(MinecraftLauncher);//TODO: Change to a default (ScriptLauncher / ShellLauncher?), or auto-identify launch method

		public IFileSystem GetFileSystem() {
			return FileSystemManager.Default.ResolveDirectory(Location).CreateView();
		}

		const string MpmDirectory = ".mpm";
		const string ConfigurationName = "configuration.json";
		readonly JsonSerializerSettings JsonSettings = new JsonSerializerSettings {
			TypeNameHandling = TypeNameHandling.Auto,
			Formatting = Formatting.Indented,
			ObjectCreationHandling = ObjectCreationHandling.Replace,
		};
		public InstanceConfiguration @Configuration {
			get {
				using (var fileSystem = GetFileSystem()) {
					var mpmDir = fileSystem.ResolveDirectory(MpmDirectory);
					if (!mpmDir.Exists) {
						return InstanceConfiguration.Empty;
					}
					var configFile = mpmDir.ResolveFile(ConfigurationName);
					if (!configFile.Exists) {
						return InstanceConfiguration.Empty;
					}
                    var configReader = configFile.GetContent().GetReader();
					using (var jsonReader = new JsonTextReader(configReader)) {
						return JsonSerializer.Create(JsonSettings).Deserialize<InstanceConfiguration>(jsonReader);
					}
				}
			}
			set {
				using (var fileSystem = GetFileSystem()) {
					var mpmDir = fileSystem.ResolveDirectory(MpmDirectory);
					if (!mpmDir.Exists) {
						mpmDir.Create(true);
					}
					var configFile = fileSystem.ResolveFile(ConfigurationName);
					configFile.ResolveDirectory(".").Create();
					var configWriter = configFile.GetContent().GetWriter();
					using (var jsonWriter = new JsonTextWriter(configWriter)) {
						JsonSerializer.Create(JsonSettings).Serialize(jsonWriter, value, typeof(InstanceConfiguration));
					}
				}
			}
		}

		public ILauncher CreateLauncher() {
			var ctor = LauncherType.GetConstructor(new Type[0]);
			return (ILauncher)ctor.Invoke(new object[0]);
		}
	}
}
