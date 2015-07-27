using System;
using NServiceKit.ServiceHost;

namespace MPM.Net.DTO {

	[Route("/packages/{PackageName}", Verbs = "GET")]
	public class PackageRequest : IReturn<Package> {
		public String PackageName { get; set; }
	}
}
