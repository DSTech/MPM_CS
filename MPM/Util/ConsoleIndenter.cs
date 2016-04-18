using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Util {
    public struct ConsoleIndenter : IDisposable {
        public readonly TextWriter OriginalOut;
        private readonly IndentedTextWriter _writer;

        public uint Depth {
            get { return _writer.Depth; }
            set { _writer.Depth = value; }
        }

        public string IndentSequence => _writer.IndentSequence;

        public ConsoleIndenter(uint depth = 1, string indentSequence = "  ") {
            this.OriginalOut = Console.Out;
            Debug.Assert(OriginalOut != null);
            this._writer = new IndentedTextWriter(OriginalOut, depth, indentSequence, leaveOpen: true);
            Debug.Assert(_writer != null);
            Console.SetOut(_writer);
        }

        public static ConsoleIndenter ByTabs => new ConsoleIndenter(1, "\t");

        #region Implementation of IDisposable

        public void Dispose() {
            Debug.Assert(OriginalOut != null);
            Console.SetOut(OriginalOut);
            _writer.Dispose();
        }

        #endregion
    }
}
