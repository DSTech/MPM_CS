using System.Linq;
using MPM.Core.Dependency;
using semver.tools;
using Xunit;

namespace MPMTest.Core.Dependency {

	public class PackageSpecTests {

		[Fact]
		public void Equality() {
			var first = new PackageSpec {
				Name = "testPackage",
				VersionSpec = new VersionSpec(SemanticVersion.Parse("0.0.1")),
				Manual = true,
			};
			var second = new PackageSpec {
				Name = "testPackage",
				VersionSpec = new VersionSpec(SemanticVersion.Parse("0.0.1")),
				Manual = true,
			};
			var third = new PackageSpec {
				Name = "testPackage",
				VersionSpec = new VersionSpec(SemanticVersion.Parse("0.0.2")),
				Manual = false,
			};
			Assert.Equal(first, second);
			Assert.True(new[] { first }.Except(new[] { second }).Count() == 0);
			Assert.NotEqual(second, third);
		}
	}
}
