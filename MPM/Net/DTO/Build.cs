using System;
using System.Collections.Generic;

namespace MPM.Net.DTO {
    public class Build {
        public String Package { get; set; }
        public List<Author> Authors { get; set; }
        public Version Version { get; set; }
        public String GivenVersion { get; set; }
        public String Arch { get; set; }
        public PackageSide Side { get; set; } = PackageSide.Universal;
        public List<InterfaceProvision> Interfaces { get; set; }
        public DependencySpec Dependencies { get; set; }
        public List<PackageConflict> Conflicts { get; set; }
        public List<String> Hashes { get; set; }
    }
}
