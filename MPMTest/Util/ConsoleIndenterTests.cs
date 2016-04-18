using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Util;
using Xunit;

namespace MPMTest.Util {
    public class ConsoleIndenterTests {
        [Fact]
        public void Indentation() {
            var sb = new StringBuilder();
            using (var writer = new StringBuilderWriter(sb)) {
                var oldOut = Console.Out;
                try {
                    Console.SetOut(writer);
                    const string testString = " rawr ";
                    const string indentSequence = "IND";
                    const uint indentCount = 3;

                    using (new ConsoleIndenter(indentCount, indentSequence)) {
                        Console.WriteLine(testString);
                        Console.Out.Flush();
                        Assert.Equal($"{indentSequence.RepeatStr(indentCount)}{testString}{Environment.NewLine}", sb.ToString());
                        sb.Clear();
                        using (new ConsoleIndenter(indentCount, indentSequence)) {
                            Console.WriteLine(testString);
                            Console.Out.Flush();
                            Assert.Equal($"{indentSequence.RepeatStr(indentCount * 2)}{testString}{Environment.NewLine}", sb.ToString());
                        }
                    }
                } finally {
                    Console.SetOut(oldOut);
                }
            }
        }
    }
}
