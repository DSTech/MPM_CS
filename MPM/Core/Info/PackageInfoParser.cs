using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances;

namespace MPM.Core.Info {
	/// <summary>
	/// Exposes the internal-structure translations of the package info through use of translators.
	/// Returns defaults for missing keys.
	/// </summary>
	public class PackageInfoParser {
		private readonly PackageJsonParser packageJsonParser;
		public PackageInfoParser(string packageJson) {
			this.packageJsonParser = new PackageJsonParser(packageJson);
		}
		public IEnumerable<IInstallationOperation> InstallationScript {
			get {
				return InstallationScriptParser.Parse(packageJsonParser.InstallationScript);
			}
		}
	}
}
