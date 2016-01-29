namespace MPM.Types {
    [Newtonsoft.Json.JsonConverter(typeof(MPM.Util.Json.VersionRangeConverter))]
    public class SemRange : SemVer.Range {
        private readonly string _rangeSpec;

        public SemRange(string rangeSpec, bool loose = false) : base(rangeSpec, loose) {
            this._rangeSpec = rangeSpec;
        }

        public override string ToString() {
            return _rangeSpec;
        }
    }
}
