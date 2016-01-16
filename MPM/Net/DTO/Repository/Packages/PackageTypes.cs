using System.Collections.Generic;
using NServiceKit;
using NServiceKit.ServiceHost;

namespace MPM.Net.DTO.Repository.Packages {

    [Route("/builds")]
    [Route("/builds/{" + nameof(PackageNames) + "}")]
    public class BuildListRequest : IReturn<List<MPM.Net.DTO.Build>> {
        public string[] PackageNames { get; set; }
        public BuildListRequest(params string[] packageNames) {
            this.PackageNames = packageNames;
        }
    }

    [Route("/builds", "POST")]
    [Route("/builds/{" + nameof(PackageName) + "}", "PUT")]
    public class BuildSubmission : IReturn<MPM.Net.DTO.Build> {
        public string PackageName { get; set; }
        public MPM.Net.DTO.Build BuildInfo { get; set; }
    }

    [Route("/builds/{" + nameof(PackageName) + "}", "GET")]
    [Route("/builds/{" + nameof(PackageName) + "}", "DELETE")]
    public class BuildRequest : IReturn<IList<MPM.Net.DTO.Build>> {
        public BuildRequest() {
        }
        public BuildRequest(string packageName) {
            this.PackageName = packageName;
        }
        public string PackageName { get; set; }
    }
}
