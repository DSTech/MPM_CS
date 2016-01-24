using System;
using System.Diagnostics;
using MPM.Types;

namespace MPM.Net.Protocols.Minecraft.Types {

    /// <summary>
    /// Contains native-library inclusions for each particular operating system, with an optional ${bitness} placeholder in circumstances where bitness matters
    /// </summary>
    /// <example>
    /// {
    ///  "linux": "natives-linux",
    ///  "windows": "natives-windows-${bitness}",
    ///  "osx": "natives-osx",
    /// }
    /// </example>
    /// <seealso cref="http://wiki.vg/Game_Files"/>
    public class LibraryNativesSpec {
        public LibraryNativesSpec() {
        }

        public LibraryNativesSpec(string windows, string linux, string osx) {
            this.Windows = windows ?? "natives-windows";
            this.Linux = linux ?? "natives-linux";
            this.Osx = osx ?? "natives-osx";
        }

        //Tells which file "natives-spec" to download on the given platform, or returns null if none need downloaded.
        public string AppliedTo(PlatformID platform, bool x64 = true) {
            string platformStr;
            switch (platform) {
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE: {
                        const string msg = "This system isn't even supported by .NET.";
                        Debug.Assert(false, msg);
                        throw new NotSupportedException(msg);
                    }
                case PlatformID.Xbox: {
                        const string msg = "How are you even running this?";
                        Debug.Assert(false, msg);
                        throw new NotSupportedException();
                    }
                case PlatformID.Win32NT:
                    if (String.IsNullOrWhiteSpace(Windows)) {
                        return null;
                    }
                    platformStr = Windows;
                    break;
                case PlatformID.Unix:
                    if (String.IsNullOrWhiteSpace(Linux)) {
                        return null;
                    }
                    platformStr = Linux;
                    break;
                case PlatformID.MacOSX:
                    platformStr = Osx;
                    break;
                default:
                    throw new NotSupportedException();
            }
            if (platformStr.Contains("${platform}")) {
                return platformStr.Replace("${platform}", (x64 ? "64" : "32"));
            }
            return platformStr;
        }

        ///<summary>
        /// See info for <seealso cref="LibraryNativesSpec"/>
        ///</summary>
        public string Windows { get; set; }

        ///<summary>
        /// See info for <seealso cref="LibraryNativesSpec"/>
        ///</summary>
        public string Linux { get; set; }

        ///<summary>
        /// See info for <seealso cref="LibraryNativesSpec"/>
        ///</summary>
        public string Osx { get; set; }
    }
}
