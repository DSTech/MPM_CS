using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MPMTest {
	public class InstallationScriptTests {
		private StreamReader FetchTestScript() {
			return new StreamReader(File.OpenRead(Path.Combine(".", "TestResources", "InstallationScript.json")));
        }
		[Fact]
		public void ParseScript() {
			using (var testScript = FetchTestScript()) {
				Debug.WriteLine("Woo!");
			}
		}
	}
}
