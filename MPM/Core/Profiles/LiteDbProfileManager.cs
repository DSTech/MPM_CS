using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using MPM.Data;

namespace MPM.Core.Profiles {
	public class LiteDbProfileManager : IProfileManager, IDisposable {
		public readonly LiteDatabase Db;
		public readonly string ProfileCollectionName;
		private LiteCollection<MutableProfile> ProfileCollection => Db.GetCollection<MutableProfile>(ProfileCollectionName);

		public LiteDbProfileManager(LiteDatabase db, string profileCollectionName) {
			if ((this.Db = db) == null) {
				throw new ArgumentNullException(nameof(db));
			}
			if ((this.ProfileCollectionName = profileCollectionName) == null) {
				throw new ArgumentNullException(nameof(profileCollectionName));
			}
		}

		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}


		private bool disposed = false;
		protected virtual void Dispose(bool disposing) {
			if (!disposing || disposed) {
				return;
			}
			Db.Dispose();
			disposed = true;
		}

		public IEnumerable<IProfile> Entries => ProfileCollection.FindAll();

		public IEnumerable<Guid> Ids => ProfileCollection.FindAll().Select(x => x.Id);

		public void Clear() => ProfileCollection.Delete(Query.All());

		public bool Contains(Guid profileId) => Fetch(profileId) != null;

		public void Delete(Guid profileId) => ProfileCollection.Delete(profileId);

		public IProfile Fetch(Guid profileId) => ProfileCollection.FindById(profileId);

		public void Store(IProfile profileData) {
			var mutableProfile = profileData.ToMutableProfile();
			ProfileCollection.Upsert(mutableProfile);
		}
	}
}
