using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Net.DTO;
using MPM.Core;
using Refit;

namespace MPM.Net {
	public interface IRepositoryAPI {
		[Get("/")]
		Task<IEnumerable<Package>> GetPackages();

		[Get("/{packageName}")]
		Task<Package> GetPackage(string packageName);

		[Get("/{packageName}/{buildVersion}")]
		Task<Build> GetBuild(string packageName, VersionIdentifier buildVersion);
	}
}
