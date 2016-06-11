using System.Collections.Generic;
using System.Text;

namespace System {
    public static class StringX {
        public static bool IsNullOrWhiteSpace(this String @string) => String.IsNullOrWhiteSpace(@string);
        public static string TransformChars(this String str, Func<char, char> charModifier) {
            var sb = new StringBuilder(str.Length);
            foreach (var c in str) {
                sb.Append(charModifier(c));
            }
            return sb.ToString();
        }
        public static string Repeat(this String str, uint count) {
            if (count == 0) { return String.Empty; }
            var sb = new StringBuilder(str.Length * (int)count);
            for (uint i = 0; i < count; ++i) { sb.Append(str); }
            return sb.ToString();
        }

        // ReSharper disable once InvokeAsExtensionMethod
        public static string RepeatStr(this String str, uint count) => StringX.Repeat(str, count);

        public static string Join(this IEnumerable<string> subject, string separator) => String.Join(separator, subject);
    }
}
