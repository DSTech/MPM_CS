using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MPM.Core.Instances.Info {
    /// <summary>
    ///     Exposes the primative JSON objects from the package info file in preparation for translation to internal classes.
    ///     Also returns null for missing keys.
    /// </summary>
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
                } catch (Exception) {
                    return null;
                }
            }
        }
    }
}
