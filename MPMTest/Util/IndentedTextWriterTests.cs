using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Util;
using Xunit;

namespace MPMTest.Util {
    public class IndentedTextWriterTests {
        [Fact]
        public void WithStringBuilderWriter() {
            var strBld = new StringBuilder();
            using (var writer = new StringBuilderWriter(strBld)) {
                writer.WriteLine(" abc ");
                Assert.Equal($" abc {Environment.NewLine}", strBld.ToString());
            }
        }

        [Fact]
        public void StringBuilderWriterClearFlush() {
            var strBld = new StringBuilder();
            using (var writer = new StringBuilderWriter(strBld)) {
                writer.WriteLine(" abc defghijkl \t ");
                writer.WriteLine();
                writer.WriteLine("test");
                strBld.Clear();
                Assert.Equal("", strBld.ToString());
            }
            Assert.Equal("", strBld.ToString());
        }

        [Fact]
        public void Indenting() {
            const string indentSequence = "\t\t";
            const int indentCount = 7;
            const string testStr = " abc \t ";
            var strBld = new StringBuilder();
            using (var writer = new StringBuilderWriter(strBld)) {
                using (var indenter = new IndentedTextWriter(writer, indentCount, indentSequence)) {
                    indenter.WriteLine(testStr);
                    Assert.Equal($"{indentSequence.RepeatStr(indentCount)}{testStr}{Environment.NewLine}", strBld.ToString());
                }
            }
        }

        [Fact]
        public void VariableIndenting() {
            const string indentSequence = "\t\t";
            const int baseIndentCount = 3;
            const string testStr = " abc \t ";
            var strBld = new StringBuilder();
            using (var writer = new StringBuilderWriter(strBld)) {
                using (var indenter = new IndentedTextWriter(writer, baseIndentCount, indentSequence)) {
                    indenter.Indent();
                    indenter.WriteLine(testStr);
                    indenter.Dedent();
                    indenter.WriteLine(testStr);
                    var result = strBld.ToString().Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                    Assert.Equal(3, result.Length);
                    Assert.Equal($"{indentSequence.RepeatStr(baseIndentCount + 1)}{testStr}", result[0]);
                    Assert.Equal($"{indentSequence.RepeatStr(baseIndentCount)}{testStr}", result[1]);
                    Assert.Equal(String.Empty, result[2]);
                }
            }
        }

        [Fact]
        public void NestedIndenting() {
            const string indentSequence = "\t\t";
            const string indentSequenceNested = " ";
            const int indentCount = 7;
            const int indentCountNested = 137;
            const string testStr = " abc \t ";
            var strBld = new StringBuilder();
            using (var writer = new StringBuilderWriter(strBld)) {
                using (var nested = new IndentedTextWriter(new IndentedTextWriter(writer, indentCount, indentSequence), indentCountNested, indentSequenceNested) { LeaveOpen = false }) {
                    nested.WriteLine(testStr);
                    Assert.Equal($"{indentSequence.RepeatStr(indentCount)}{indentSequenceNested.RepeatStr(indentCountNested)}{testStr}{Environment.NewLine}", strBld.ToString());
                }
            }
        }
    }
}
