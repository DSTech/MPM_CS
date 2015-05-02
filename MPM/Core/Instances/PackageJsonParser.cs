using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MPM.Core.Instances {
	public class PackageJsonParser {
		private JObject packageInfo;
		public PackageJsonParser(string packageJson) {
			this.packageInfo = JObject.Parse(packageJson);
		}
		public JObject[] InstallationScript {
			get {
				try {
					return packageInfo
						.Property("installation")
						.AsJEnumerable()
						.Select(tok => tok.ToObject<JObject>())
						.ToArray();
				} catch {
					return null;
				}
			}
		}
	}
}
