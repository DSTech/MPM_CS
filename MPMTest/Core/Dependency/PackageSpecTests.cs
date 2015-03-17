using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPM;
using MPM.Core.Archival;
using MPM.Core.Dependency;
using System.Linq;

namespace MPMTest {
	[TestClass]
	public class PackageSpecTests {
		[TestMethod]
		public void Equality() {
			var first = new PackageSpec {
				Name = "testPackage",
				Version = semver.tools.SemanticVersion.Parse("0.0.1"),
				Manual = true,
			};
			var second = new PackageSpec {
				Name = "testPackage",
				Version = semver.tools.SemanticVersion.Parse("0.0.1"),
				Manual = true,
			};
			var third = new PackageSpec {
				Name = "testPackage",
				Version = semver.tools.SemanticVersion.Parse("0.0.2"),
				Manual = false,
			};
			Assert.AreEqual(first, second);
			Assert.IsTrue(new[] { first }.Except(new[] { second }).Count() == 0);
			Assert.AreNotEqual(second, third);
		}
		[TestMethod]
		public void Comparison() {
			var first = new PackageSpec {
				Name = "testPackage",
				Version = semver.tools.SemanticVersion.Parse("0.0.1"),
				Manual = true,
			};
			var second = new PackageSpec {
				Name = "testPackage",
				Version = semver.tools.SemanticVersion.Parse("0.0.2"),
				Manual = true,
			};
			var third = new PackageSpec {
				Name = "testPackage",
				Version = semver.tools.SemanticVersion.Parse("0.0.2"),
				Manual = false,
			};
			Assert.AreEqual(first.Name, second.Name);
			Assert.AreEqual(first.Manual, second.Manual);
			Assert.IsTrue(first < second, "Where other values are equal, packages should compare by version");
			Assert.AreEqual(second.Name, third.Name);
			Assert.AreEqual(second.Version, third.Version);
			Assert.IsTrue(second < third, "Where other values are equal, manually selected packages should appear be first in an ordering");
		}
	}
}
