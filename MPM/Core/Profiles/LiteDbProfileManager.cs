using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using MPM.Data;

namespace MPM.Core.Profiles {
    public class LiteDbProfileManager : IProfileManager {
        private readonly LiteCollection<MutableProfile> ProfileCollection;

        public LiteDbProfileManager(LiteCollection<MutableProfile> profileCollection) {
            if ((this.ProfileCollection = profileCollection) == null) {
                throw new ArgumentNullException(nameof(profileCollection));
            }
        }

        public IEnumerable<IProfile> Entries => ProfileCollection.FindAll();

        public IEnumerable<String> Names => ProfileCollection.FindAll().Select(x => x.Name);

        public void Clear() => ProfileCollection.Delete(Query.All());

        public bool Contains(String profileName) => Fetch(profileName) != null;

        public void Delete(String profileName) => ProfileCollection.Delete(profile => profile.Name == profileName);

        public IProfile Fetch(String profileName) => ProfileCollection.FindOne(profile => profile.Name == profileName);

        public void Store(IProfile profileData) {
            var mutableProfile = profileData.ToMutableProfile();
            ProfileCollection.Upsert(mutableProfile);
        }
    }
}
