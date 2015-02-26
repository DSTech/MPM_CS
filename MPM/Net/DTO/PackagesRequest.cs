using System;
using System.Collections.Generic;
using NServiceKit.ServiceHost;

namespace MPM.Net.DTO {
	[Route("/packages", Verbs = "GET")]
	[Route("/packages/{PackageNames}", Verbs = "GET")]
	public class PackagesRequest : IReturn<IEnumerable<Package>> {
		public String[] PackageNames { get; set; }
		public DateTime? LastSync { get; set; }
	}
}
