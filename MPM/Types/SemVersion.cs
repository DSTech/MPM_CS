namespace MPM.Types {
    [Newtonsoft.Json.JsonConverter(typeof(MPM.Util.Json.VersionConverter))]
    public class SemVersion : SemVer.Version {
        public SemVersion(string input, bool loose = false)
            : base(input, loose) {
        }

        public SemVersion(int major, int minor, int patch, string preRelease = null, string build = null)
            : base(major, minor, patch, preRelease, build) {
        }
    }
}
