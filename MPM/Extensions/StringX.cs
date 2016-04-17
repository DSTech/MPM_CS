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
    }
}
