using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Core.Instances.Cache;
using MPM.Data;

namespace MPM.Core {
	/// <summary>
	/// Should:
	///	 Provide:
	///	  Root Meta Database: <see cref="IUntypedKeyValueStore{String}"/>
	///	  Profile Store: <see cref="Func{Guid, IProfile}"/>
	///   Global Cache: <see cref="Func{ICacheManager}"/>
	/// </summary>
	public class GlobalStorage {
		public IUntypedKeyValueStore<String> FetchDataStore() {
			throw new NotImplementedException();
		}
		public IProfile FetchProfile(Guid profileId) {
			throw new NotImplementedException();
		}
		public ICacheManager FetchCacheManager() {
			throw new NotImplementedException();
		}
	}
}
