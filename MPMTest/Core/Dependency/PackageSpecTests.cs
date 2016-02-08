using System.Linq;
using MPM.Core.Dependency;
using Xunit;

namespace MPMTest.Core.Dependency {
    public class PackageSpecTests {
        [Fact]
        public void Equality() {
            var first = new PackageSpec {
                Name = "testPackage",
                VersionSpec = new MPM.Types.SemRange("0.0.1"),
                Manual = true,
            };
            var second = new PackageSpec {
                Name = "testPackage",
                VersionSpec = new MPM.Types.SemRange("0.0.1"),
                Manual = true,
            };
            var third = new PackageSpec {
                Name = "testPackage",
                VersionSpec = new MPM.Types.SemRange("0.0.2"),
                Manual = false,
            };
            Assert.Equal(first, second);
            Assert.Equal(first.GetHashCode(), second.GetHashCode());
            Assert.False(
                new PackageSpec[] { first }
                    .Except(new PackageSpec[] { second })
                    .Any()
            );
            Assert.NotEqual(second, third);
        }
    }
}
