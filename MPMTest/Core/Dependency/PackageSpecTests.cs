using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPM.Core.Dependency;
using semver.tools;

namespace MPMTest.Core.Dependency {
	[TestClass]
	public class PackageSpecTests {
		[TestMethod]
		public void Equality() {
			var first = new PackageSpec {
				Name = "testPackage",
				Version = new VersionSpec(SemanticVersion.Parse("0.0.1")),
				Manual = true,
			};
			var second = new PackageSpec {
				Name = "testPackage",
				Version = new VersionSpec(SemanticVersion.Parse("0.0.1")),
				Manual = true,
			};
			var third = new PackageSpec {
				Name = "testPackage",
				Version = new VersionSpec(SemanticVersion.Parse("0.0.2")),
				Manual = false,
			};
			Assert.AreEqual(first, second);
			Assert.IsTrue(new[] { first }.Except(new[] { second }).Count() == 0);
			Assert.AreNotEqual(second, third);
		}
	}
}
