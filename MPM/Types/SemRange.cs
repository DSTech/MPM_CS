namespace MPM.Types {
    [Newtonsoft.Json.JsonConverter(typeof(MPM.Util.Json.VersionRangeConverter))]
    public class SemRange : SemVer.Range {
        public SemRange(string rangeSpec, bool loose = false)
            : base(rangeSpec, loose) {
        }
    }
}
