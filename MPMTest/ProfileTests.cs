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
			if (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) {
				//TODO: Remove the need for this bypass.
				//Bypass exception: Community.CsharpSqlite.SQLiteClient.SqliteSyntaxException : unable to open database file
				Console.WriteLine("Bypassing test due to operating system incompatibility.");
				return;
			}
			var gstore = new GlobalStorage();
			Assert.Null(gstore.FetchProfile(Guid.Empty));
			using (var profMgr = gstore.FetchProfileManager()) {
				var testProfile = new MutableProfile(Guid.NewGuid(), "testProfile", new Dictionary<string, string> {
					["testAttribute"] = "testValue",
				});
				try {
					profMgr.Store(testProfile);//Profiles can be stored
					var guidToLoad = new Guid(testProfile.Id.ToString());//Guids can be converted to and from strings
					var loadedProfile = profMgr.Fetch(guidToLoad);//Profiles can be loaded
					Assert.Equal(testProfile.Id, loadedProfile.Id);
					Assert.Equal(testProfile.Name, loadedProfile.Name);
					foreach (var preferencePair in testProfile.Preferences) {
						Assert.Equal(preferencePair.Value, loadedProfile.Preferences[preferencePair.Key]);
					}
				} finally {
					profMgr.Delete(testProfile.Id);
				}
			}
		}
	}
}
