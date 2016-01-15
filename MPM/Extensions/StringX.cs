using System;
using System.IO;
using System.Threading.Tasks;

namespace MPM.Extensions {

    public static class StringX {
        public static bool IsNullOrWhiteSpace(this String @string) => String.IsNullOrWhiteSpace(@string);
    }
}
