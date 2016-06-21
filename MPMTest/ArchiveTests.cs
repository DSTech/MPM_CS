using System;
using System.IO;
using System.Threading.Tasks;
using MPM.Archival;
using Xunit;

namespace MPMTest {

    public class LeadingHashTests {

        [Fact]
        public void LeadingHashVerification() {
            string packageName = DateTime.Today.ToString();
            var testContent = packageName.RepeatStr(10);
            var testContentBytes = System.Text.Encoding.UTF8.GetBytes(testContent);
            using (var archived = new MemoryStream()) {
                Archive.Create(new MemoryStream(testContentBytes, false), packageName, archived, leaveSourceOpen: false);
                Assert.NotEmpty(archived.ToArrayFromStart());

                using (var extracted = new MemoryStream()) {
                    Archive.Unpack(archived.SeekToStart(), packageName, extracted, leaveSourceOpen: true);
                    var extractedArr = extracted.ToArrayFromStart();
                    Assert.NotNull(extractedArr);
                    Assert.Equal(testContentBytes, extractedArr);
                }
            }
        }
    }

    public class ArchiveTests {

        //[Fact]
        //public async Task ArchiveVerificationSplit() {
        //    const string packageName = "testingArchive";
        //    string testString = DateTime.Today.ToString();
        //    testString = testString + testString + testString + testString;//for a bit of content length
        //    var testInputValue = System.Text.Encoding.UTF8.GetBytes(testString);

        //    var archive = await Archive.CreateArchive(packageName, testInputValue, 16);
        //    Assert.NotNull(archive);

        //    var unpacked = await archive.Unpack(packageName);
        //    Assert.NotNull(unpacked);
        //    Assert.Equal(testString, System.Text.Encoding.UTF8.GetString(unpacked));
        //}

        //[Fact]
        //public async Task ArchiveVerification() {
        //    const string packageName = "testingArchive";
        //    string testString = DateTime.Today.ToString();
        //    var testInputValue = System.Text.Encoding.UTF8.GetBytes(testString);

        //    var archive = await Archive.CreateArchive(packageName, testInputValue);
        //    Assert.NotNull(archive);

        //    var unpacked = await archive.Unpack(packageName);
        //    Assert.NotNull(unpacked);
        //    Assert.Equal(testString, System.Text.Encoding.UTF8.GetString(unpacked));
        //}
    }
}
