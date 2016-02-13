using System;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MPM.Types;
using Newtonsoft.Json.Converters;
using SemVersion = MPM.Types.SemVersion;

namespace MPM.Util.Json {
    public class UtcDateTimeConverter : IsoDateTimeConverter {
        public UtcDateTimeConverter() {
            this.DateTimeStyles = DateTimeStyles.AssumeUniversal;
        }
    }
}
