using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MPM.Archival;
using MPM.Core;
using MPM.Core.Profiles;
using Xunit;

namespace MPMTest {

	public class ProfileTests {

		[Fact]
		public void ProfileSaving() {
			var gstore = new GlobalStorage();
			//Assert.Null(gstore.FetchProfile(Guid.Empty));
			var profMgr = gstore.FetchProfileManager();
			var testProfile = new MutableProfile("testProfile", new Dictionary<string, string> {
				["testAttribute"] = "testValue",
			});
			try {
				profMgr.Store(testProfile);//Profiles can be stored
				var nameToLoad = testProfile.Name;
				var loadedProfile = profMgr.Fetch(nameToLoad);//Profiles can be loaded
				Assert.Equal(testProfile.Name, loadedProfile.Name);
				foreach (var preferencePair in testProfile.Preferences) {
					Assert.Equal(preferencePair.Value, loadedProfile.Preferences[preferencePair.Key]);
				}
			} finally {
				profMgr.Delete(testProfile.Name);
			}
		}
	}
}
