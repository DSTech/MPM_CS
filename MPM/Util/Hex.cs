using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Util {
    public static class Hex {
        public static byte[] GetBytes(string hex) {
            if (hex.Length % 2 != 0) {
                throw new FormatException("Hex input was not of even length");
            }
            var result = new byte[hex.Length / 2];
            for (var i = 0; i < result.Length; i++) {
                result[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return result;
        }

        public static string GetString(byte[] bytes) {
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes) {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }
    }
}
