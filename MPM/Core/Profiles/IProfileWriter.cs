using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Profiles {
	/// <summary>
	/// Allows writing of <see cref="IProfile"/>s to a profile store.
	/// </summary>
	public interface IProfileWriter : IDisposable {
		void Store(IProfile profileData);
		void Delete(Guid profileId);
		void Clear();
	}
}
