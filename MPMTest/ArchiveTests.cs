using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MPM.Core.Archival;

namespace MPMTest {
	[TestClass]
	public class LeadingHashTests {
		[TestMethod]
		public void LeadingHashVerification() {
			string testString = DateTime.Today.ToString();
			var testInputValue = System.Text.Encoding.UTF8.GetBytes(testString);
			var hashed = Archive.ApplyLeadingHash(testInputValue);
			Assert.IsNotNull(hashed);

			var verifiedResult = Archive.VerifyLeadingHash(hashed);
			Assert.IsNotNull(verifiedResult);
			Assert.AreEqual(testString, System.Text.Encoding.UTF8.GetString(verifiedResult));
		}
	}
	[TestClass]
	public class ArchiveTests {
		[TestMethod]
		public async Task ArchiveVerificationSplit() {
			const string packageName = "testingArchive";
			string testString = DateTime.Today.ToString();
			testString = testString + testString + testString + testString;//for a bit of content length
			var testInputValue = System.Text.Encoding.UTF8.GetBytes(testString);

			var archive = await Archive.CreateArchive(packageName, testInputValue, 16);
			Assert.IsNotNull(archive);

			var unpacked = await archive.Unpack(packageName);
			Assert.IsNotNull(unpacked);
			Assert.AreEqual(testString, System.Text.Encoding.UTF8.GetString(unpacked));
		}

		[TestMethod]
		public async Task ArchiveVerification() {
			const string packageName = "testingArchive";
			string testString = DateTime.Today.ToString();
			var testInputValue = System.Text.Encoding.UTF8.GetBytes(testString);

			var archive = await Archive.CreateArchive(packageName, testInputValue);
			Assert.IsNotNull(archive);

			var unpacked = await archive.Unpack(packageName);
			Assert.IsNotNull(unpacked);
			Assert.AreEqual(testString, System.Text.Encoding.UTF8.GetString(unpacked));
		}
	}
}
