using System.Linq;
using MPM.Core.Dependency;
using Xunit;

namespace MPMTest.Core.Dependency {
    public class PackageSpecTests {
        [Fact]
        public void VersionRangeEquality() {
            var first = new MPM.Types.SemRange("0.0.1");
            var second = new MPM.Types.SemRange("0.0.1");
            var third = new MPM.Types.SemRange("0.0.2");

            Assert.Equal(first, second);
            Assert.True(Equals(first, second));
            Assert.Equal(second, first);
            Assert.Equal(third, third);
            Assert.NotEqual(first, null);
            Assert.NotEqual(first, third);
            Assert.NotEqual(third, first);
        }
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
            Assert.Equal(first.Arch, second.Arch);
            Assert.Equal(first.Manual, second.Manual);
            Assert.Equal(first.Name, second.Name);
            Assert.Equal(first.Side, second.Side);
            Assert.Equal(first.VersionSpec, second.VersionSpec);
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
