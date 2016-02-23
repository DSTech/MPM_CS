using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Types;
using Xunit;

namespace MPMTest.Types {
    public class HashTests {
        [Fact]
        public void HashEquality() {
            var a = new Hash("bla", new byte[] { 42, 13 });
            var b = new Hash("bla", new byte[] { 42, 13 });
            var c = new Hash("bla", new byte[] { 13, 42 });
            var d = new Hash("alb", new byte[] { 42, 13 });
            var e = new Hash("alb", new byte[] { 42, 13, 26 });

            Assert.Equal(a, b);

            Assert.Equal<char>(a.Algorithm, b.Algorithm);
            Assert.Equal<byte>(a.Checksum, b.Checksum);
            Assert.Equal(b, a);

            Assert.Equal<char>(a.Algorithm, c.Algorithm);
            Assert.NotEqual<byte>(a.Checksum, c.Checksum);
            Assert.NotEqual(a, c);

            Assert.NotEqual<char>(a.Algorithm, d.Algorithm);
            Assert.Equal<byte>(a.Checksum, d.Checksum);
            Assert.NotEqual(a, d);

            Assert.NotEqual<char>(c.Algorithm, d.Algorithm);
            Assert.NotEqual<byte>(c.Checksum, d.Checksum);
            Assert.NotEqual(c, d);

            Assert.Equal<char>(d.Algorithm, e.Algorithm);
            Assert.NotEqual<byte>(d.Checksum, e.Checksum);
            Assert.NotEqual(d, e);
        }
    }
}
