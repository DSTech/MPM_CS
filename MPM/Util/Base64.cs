using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Extensions;

namespace MPM.Util {
    public static class Base64 {
        public static string GetString(byte[] bytes) {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] GetBytes(string standardB64) {
            return Convert.FromBase64String(standardB64);
        }

        public static string GetSafeString(byte[] bytes) {
            return ToSafe(Convert.ToBase64String(bytes));
        }

        public static byte[] GetSafeBytes(string safeB64) {
            return Convert.FromBase64String(ToStandard(safeB64));
        }

        public static string ToStandard(string safeB64) {
            //if (safeB64.Length % 4 == 3) {
            //    throw new ArgumentException("contains invalid number of characters", nameof(safeB64));
            //}
            return safeB64
                .TransformChars(c => {
                    switch (c) {
                        case '-':
                            return '+';
                        case '_':
                            return '/';
                        default:
                            return c;
                    }
                })
                .PadRight(safeB64.Length + (4 - safeB64.Length % 4) % 4, '=');
        }

        public static string ToSafe(string standardB64) {
            return standardB64
                .TransformChars(c => {
                    switch (c) {
                        case '+':
                            return '-';
                        case '/':
                            return '_';
                        default:
                            return c;
                    }
                })
                .TrimEnd('=');
        }

        public static byte[] GetBytesUnknown(string unknownB64) {
            byte[] checksum;
            if (unknownB64.Contains('+') || unknownB64.Contains('/') || unknownB64.EndsWith("=")) {
                checksum = Base64.GetBytes(unknownB64);
            } else {
                checksum = Base64.GetSafeBytes(unknownB64);
            }
            return checksum;
        }
    }
}
