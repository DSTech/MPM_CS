using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPM.Core.Profiles {

	/// <summary>
	/// Allows reading of <see cref="IProfile"/>s from a profile store.
	/// </summary>
	public interface IProfileReader {
		IEnumerable<IProfile> Entries { get; }
		IEnumerable<String> Names { get; }

		bool Contains(String name);

		IProfile Fetch(String name);
	}
}
