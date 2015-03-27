using System;
using System.Threading.Tasks;
using MPM.Core.Archival;
using Xunit;

namespace MPMTest {
	public class LeadingHashTests {
		[Fact]
		public void LeadingHashVerification() {
			string testString = DateTime.Today.ToString();
			var testInputValue = System.Text.Encoding.UTF8.GetBytes(testString);
			var hashed = Archive.ApplyLeadingHash(testInputValue);
			Assert.NotNull(hashed);

			var verifiedResult = Archive.VerifyLeadingHash(hashed);
			Assert.NotNull(verifiedResult);
			Assert.Equal(testString, System.Text.Encoding.UTF8.GetString(verifiedResult));
		}
	}
	public class ArchiveTests {
		[Fact]
		public async Task ArchiveVerificationSplit() {
			const string packageName = "testingArchive";
			string testString = DateTime.Today.ToString();
			testString = testString + testString + testString + testString;//for a bit of content length
			var testInputValue = System.Text.Encoding.UTF8.GetBytes(testString);

			var archive = await Archive.CreateArchive(packageName, testInputValue, 16);
			Assert.NotNull(archive);

			var unpacked = await archive.Unpack(packageName);
			Assert.NotNull(unpacked);
			Assert.Equal(testString, System.Text.Encoding.UTF8.GetString(unpacked));
		}

		[Fact]
		public async Task ArchiveVerification() {
			const string packageName = "testingArchive";
			string testString = DateTime.Today.ToString();
			var testInputValue = System.Text.Encoding.UTF8.GetBytes(testString);

			var archive = await Archive.CreateArchive(packageName, testInputValue);
			Assert.NotNull(archive);

			var unpacked = await archive.Unpack(packageName);
			Assert.NotNull(unpacked);
			Assert.Equal(testString, System.Text.Encoding.UTF8.GetString(unpacked));
		}
	}
}
