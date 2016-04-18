using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MPM.Util {
    public class IndentedTextWriter : TextWriter {

        public bool IndentNext { get; private set; }
        public bool LeaveOpen { get; set; } = false;

        public TextWriter Parent { get; }
        public string IndentSequence { get; }

        public uint Depth { get; set; }

        public IndentedTextWriter([NotNull] TextWriter parent, uint depth = 1, [NotNull] string indentSequence = "  ", bool leaveOpen = false) {
            if (parent == null) { throw new ArgumentNullException(nameof(parent)); }
            if (indentSequence == null) { throw new ArgumentNullException(nameof(indentSequence)); }
            this.Parent = parent;
            this.Depth = depth;
            this.IndentSequence = indentSequence;
            this.IndentNext = true;
            this.LeaveOpen = leaveOpen;
        }

        public override void Write(char value) {
            if (IndentNext) {
                for (var i = 0; i < Depth; ++i) { Parent.Write(IndentSequence); }
                IndentNext = false;
            }
            Parent.Write(value);
            if (value == '\n') {
                IndentNext = true;
            }
        }

        public uint Indent() => Depth < uint.MaxValue ? ++Depth : Depth;

        public uint Dedent() => Depth > 0 ? --Depth : Depth;

        public override Encoding Encoding => Parent.Encoding;

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            Parent?.Dispose();
        }
    }
}
