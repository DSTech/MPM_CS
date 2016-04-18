using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Util {
    public static class Huminz {
        public static string ByteSize(int bytes) => ByteSize((long)bytes);

        public static string ByteSize(long bytes) {
            if (bytes == 0) {
                return "0 bytes";
            }
            var orders = Convert.ToByte(Math.Floor(Math.Log(bytes, 1024)));
            var ground = Math.Pow(1024, orders);
            var dec = (double)bytes / (double)ground;
            string prefix;
            switch (orders) {
                case 0:
                    prefix = "";
                    break;
                case 1:
                    prefix = "kilo";
                    break;
                case 2:
                    prefix = "mega";
                    break;
                case 3:
                    prefix = "giga";
                    break;
                case 4:
                    prefix = "tera";
                    break;
                default:
                    prefix = $"* (1024^{orders}) ";
                    break;
            }
            return $"{dec:0.00} {prefix}bytes";
        }

        public static string ByteSizeShort(int bytes) => ByteSizeShort((long) bytes);

        public static string ByteSizeShort(long bytes) {
            if (bytes == 0) {
                return "0b";
            }
            var orders = Convert.ToByte(Math.Floor(Math.Log(bytes, 1024)));
            var ground = Math.Pow(1024, orders);
            var dec = (double)bytes / (double)ground;
            string suffix;
            switch (orders) {
                case 1:
                    suffix = "kb";
                    break;
                case 2:
                    suffix = "mb";
                    break;
                case 3:
                    suffix = "gb";
                    break;
                case 4:
                    suffix = "tb";
                    break;
                case 0:
                default:
                    return $"{bytes}b";
            }
            return $"{dec:0.00}{suffix}";
        }
    }
}
