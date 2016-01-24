using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances;
using MPM.Core.Instances.Installation;
using MPM.Core.Instances.Installation.Scripts;

namespace MPM.Core.Instances.Info {
    /// <summary>
    ///     Exposes the internal-structure translations of the package info through use of translators.
    ///     Returns defaults for missing keys.
    /// </summary>
    public class PackageInfoParser {
        private readonly PackageJsonParser packageJsonParser;

        public PackageInfoParser(string packageJson) {
            this.packageJsonParser = new PackageJsonParser(packageJson);
        }

        public IEnumerable<IFileDeclaration> InstallationScript {
            get { throw new NotImplementedException(); }
        }
    }
}
