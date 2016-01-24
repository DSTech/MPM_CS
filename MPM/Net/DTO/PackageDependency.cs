using System;

namespace MPM.Net.DTO {
    public class PackageDependency {
        public String Name { get; set; }
        public VersionSpec Version { get; set; }
        public PackageSide Side { get; set; }
    }
}
