using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MPM.Core;
using MPM.Core.Archival;
using MPM.Core.Profiles;
using Xunit;

namespace MPMTest {
	public class ProfileTests {
		[Fact]
		public void ProfileSaving() {
			var gstore = new GlobalStorage();
			Assert.Null(gstore.FetchProfile(Guid.Empty));
			using (var profMgr = gstore.FetchProfileManager()) {
				var testProfile = new MutableProfile(Guid.NewGuid(), "testProfile", new Dictionary<string, string> {
					["testAttribute"] = "testValue",
				});
				profMgr.Store(testProfile);//Profiles can be stored
				var guidToLoad = new Guid(testProfile.Id.ToString());//Guids can be converted to and from strings
				var loadedProfile = profMgr.Fetch(guidToLoad);//Profiles can be loaded 
				Assert.Equal(testProfile.Id, loadedProfile.Id);
				Assert.Equal(testProfile.Name, loadedProfile.Name);
				foreach (var preferencePair in testProfile.Preferences) {
					Assert.Equal(preferencePair.Value, loadedProfile.Preferences[preferencePair.Key]);
				}
			}
		}
	}
}
