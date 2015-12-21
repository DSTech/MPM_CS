using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Newtonsoft.Json;
using MPM.Extensions;
using LiteDB;
using MPM.Data;
using MPM.Core.Profiles;
using System.IO;
using System.Threading;
using MPM.Data.Repository;
using MPM.Types;

namespace MPMTest.Data {

    public class RepositoryTests {
        private readonly ITestOutputHelper output;

        public RepositoryTests(ITestOutputHelper output) {
            this.output = output;
        }

        private static Build testBuild = new Build(
            "testPackage",
            Enumerable.Empty<Author>(),
            new semver.tools.SemanticVersion(0, 0, 0),
            "testVersion",
            new Arch("test"),
            CompatibilityPlatform.Universal,
            CompatibilitySide.Universal,
            Enumerable.Empty<InterfaceProvision>(),
            Enumerable.Empty<InterfaceDependency>(),
            Enumerable.Empty<PackageDependency>(),
            Enumerable.Empty<Conflict>(),
            Enumerable.Empty<Hash>(),
            stable: false,
            recommended: false
        );

        [Fact]//TODO: Figure out why async unit tests are absolutely terrible, and how to address it
        public async Task RepositoryTest() {
            var dbFilePath = "./testRepository.litedb";
            if (File.Exists(dbFilePath)) {
                File.Delete(dbFilePath);
                Thread.Sleep(TimeSpan.FromSeconds(0.05));
            }
            using (var repo = new RepositoryServer(dbFilePath)) {
                repo.Packages.RegisterPackage("testPackage", new Author[] { new Author("testAuthor", "testAuthor@testSide.test") });
                repo.Packages.RegisterBuild(testBuild);
                repo.Packages.RegisterBuild(testBuild);
                var packageList = repo.Packages.FetchPackageList().ToArray();
                Assert.True(packageList.Length == 1);
                foreach (var package in packageList) {
                    output.WriteLine(package.ToString());
                }

                var testHash = Encoding.UTF8.GetBytes("testHash");
                var testHashContents = Encoding.UTF8.GetBytes("testHashContents");

                Assert.Null(repo.Hashes.Resolve(testHash).Retrieve());
                {
                    var contentStream = new MemoryStream(testHashContents);
                    repo.Hashes.Register(testHash, contentStream);
                }
                var hashContentFetcher = repo.Hashes.Resolve(testHash);
                var hashContents = await hashContentFetcher.Retrieve();
                Assert.Equal(testHashContents, hashContents);
            }
            if (File.Exists(dbFilePath)) {
                File.Delete(dbFilePath);
            }
        }
    }
}
