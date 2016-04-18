using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace MPM.Util {
    public class StringBuilderWriter : TextWriter {
        public readonly StringBuilder @StringBuilder;

        public StringBuilderWriter([NotNull] StringBuilder stringBuilder) {
            if (stringBuilder == null) throw new ArgumentNullException(nameof(stringBuilder));
            this.@StringBuilder = stringBuilder;
        }

        public override Encoding Encoding => Encoding.Unicode;

        public override void Write(char value) {
            this.@StringBuilder.Append(value);
        }
    }
}
