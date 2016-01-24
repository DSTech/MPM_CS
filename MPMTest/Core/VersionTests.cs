using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM;
using Version = MPM.Net.DTO.Version;
using Xunit;

namespace MPMTest.Core {
    public class VersionTests {
        [Fact]
        public void VersionEqualityTest() {
            var ve1 = new Version { Major = 16, Minor = 4, Patch = 3 };
            var ve2 = new Version { Major = 16, Minor = 4, Patch = 3 };
            var vue = new Version { Major = 4, Minor = 3, Patch = 16 };

            Assert.Equal(ve1, ve2);
            Assert.True(ve1 == ve2);
            Assert.False(ve1 != ve2);
            Assert.True(ve1.GetHashCode() == ve2.GetHashCode());
            Assert.True(ve1.Equals(ve2));

            Assert.Equal(ve2, ve1);
            Assert.True(ve2 == ve1);
            Assert.False(ve2 != ve1);
            Assert.True(ve2.GetHashCode() == ve1.GetHashCode());
            Assert.True(ve2.Equals(ve1));

            Assert.NotEqual(ve1, vue);
            Assert.True(ve1 != vue);
            Assert.False(ve1 == vue);
            Assert.True(vue.GetHashCode() != ve1.GetHashCode());
            Assert.False(ve1.Equals(vue));
            Assert.False(vue.Equals(ve1));
        }
    }
}
