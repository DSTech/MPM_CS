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

        public void Delete(String profileName) {
            var matchingProfName = FindNameIgnoreCase(profileName);
            if (matchingProfName == null) {
                return;
            }
            ProfileCollection.Delete(matchingProfName);
        }

        public IProfile Fetch(String profileName) {
            var matchingProfName = FindNameIgnoreCase(profileName);
            if (matchingProfName == null) {
                return null;
            }
            return ProfileCollection.FindOne(prof => prof.Name == matchingProfName);
        }

        private string FindNameIgnoreCase(string profileName) {
            return Names.FirstOrDefault(name => String.Equals(name, profileName, StringComparison.InvariantCultureIgnoreCase));
        }

        public void Store(IProfile profileData) {
            var mutableProfile = profileData.ToMutableProfile();
            ProfileCollection.Upsert(mutableProfile);
        }
    }
}
