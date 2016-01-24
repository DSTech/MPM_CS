using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Net.Protocols.Minecraft.Types {
    public static class ReleaseTypeEx {
        public static string ToDTO(this ReleaseType releaseType) {
            switch (releaseType) {
                case ReleaseType.Release:
                    return "release";
                case ReleaseType.Snapshot:
                    return "snapshot";
                case ReleaseType.OldBeta:
                    return "old_beta";
                case ReleaseType.OldAlpha:
                    return "old_alpha";
                default:
                    throw new NotSupportedException();
            }
        }

        public static ReleaseType FromString(string releaseType) {
            switch (releaseType.ToLowerInvariant()) {
                case "release":
                    return ReleaseType.Release;
                case "snapshot":
                    return ReleaseType.Snapshot;
                case "old_beta":
                    return ReleaseType.OldBeta;
                case "old_alpha":
                    return ReleaseType.OldAlpha;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public enum ReleaseType {
        Release,//"release"
        Snapshot,//"snapshot"
        OldAlpha,//"old_alpha"
        OldBeta,//"old_beta"
    }
}
