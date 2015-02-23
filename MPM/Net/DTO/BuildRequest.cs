using MPM.Core;
using NServiceKit.ServiceHost;

namespace MPM.Net.DTO {
	[Route("/{PackageName}", Verbs = "GET")]
	[Route("/{PackageName}/{BuildVersions}", Verbs = "GET")]
	public class BuildRequest : IReturn<Package> {
		public string PackageName { get; set; }
		public VersionSpecification[] BuildVersions { get; set; }
	}
}
