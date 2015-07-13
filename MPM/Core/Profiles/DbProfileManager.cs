using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MPM.Data;

namespace MPM.Core.Profiles {
	public class KeyValueStoreProfileManager : IProfileManager, IDisposable {
		private readonly ITypedKeyValueStore<Guid, IProfile> store;
		public KeyValueStoreProfileManager(ITypedKeyValueStore<Guid, IProfile> store) {
			this.store = store;
		}
		public void Dispose() => Dispose(true);
		bool disposed = false;
		protected virtual void Dispose(bool disposing) {
			if (!disposing || disposed) {
				return;
			}
			disposed = true;
			store.Dispose();
		}

		public IEnumerable<IProfile> Entries => store.Values;

		public IEnumerable<Guid> Ids => store.Keys;

		public void Clear() => store.Clear();

		public bool Contains(Guid profileId) => Fetch(profileId) != null;

		public void Delete(Guid profileId) => store.Clear(profileId);

		public IProfile Fetch(Guid profileId) => store.Get(profileId);

		public void Store(IProfile profileData) => store.Set(profileData.Id, profileData);
	}
}
