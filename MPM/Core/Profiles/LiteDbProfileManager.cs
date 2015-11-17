using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using MPM.Data;

namespace MPM.Core.Profiles {
	public class LiteDbProfileManager : IProfileManager {
		private readonly LiteCollection<MutableProfile> ProfileCollection;

		public LiteDbProfileManager(LiteCollection<BsonDocument> profileCollection) {
			if ((this.ProfileCollection = profileCollection.Database.GetCollection<MutableProfile>(profileCollection.Name)) == null) {
				throw new ArgumentNullException(nameof(profileCollection));
			}
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
