using System;
using System.Diagnostics;
using System.IO;
using Xunit;
using Newtonsoft.Json;
using MPM.Net.Protocols.Minecraft.ProtocolTypes;
using System.Linq;
using Xunit.Abstractions;
using MPM.Extensions;
using MPMTest.TestUtilities;
using Newtonsoft.Json.Linq;

namespace MPMTest.Net.Protocols.Minecraft {
    public class MinecraftVersionDetails {
        private readonly ITestOutputHelper output;
        private ITestOutputHelper Console => output;

        public MinecraftVersionDetails(ITestOutputHelper output) {
            this.output = output;
        }

        [Fact]
        [Trait("Category", "Serialization")]
        public void VersionDetails_1_8_9() {
            var testMinecraftDetailsFile = TestResources.ResourceDirectory.SubFile("MinecraftDetails_1.8.9.json");
            var originalText = File.ReadAllText(testMinecraftDetailsFile.FullName);
            var details = JsonConvert.DeserializeObject<MPM.Net.Protocols.Minecraft.ProtocolTypes.MinecraftVersion>(originalText);
            var detailsReserialized = JsonConvert.SerializeObject(details, Formatting.Indented);
            var origJsonRaw = JToken.Parse(originalText);
            var reserializedJsonRaw = JToken.Parse(detailsReserialized);
            Console.WriteLine(detailsReserialized);
            if (!JToken.DeepEquals(origJsonRaw, reserializedJsonRaw)) {
                var tempOutput = new FileInfo(Path.GetTempFileName());
                File.WriteAllText(tempOutput.FullName, detailsReserialized);
                TestResources.LaunchOutputComparison(testMinecraftDetailsFile, tempOutput);
                Assert.True(false);
            }
        }

        [Fact]
        [Trait("Category", "Serialization")]
        public void VersionDetails_1_7_10() {
            var testMinecraftDetailsFile = TestResources.ResourceDirectory.SubFile("MinecraftDetails_1.7.10.json");
            var originalText = File.ReadAllText(testMinecraftDetailsFile.FullName);
            var details = JsonConvert.DeserializeObject<MPM.Net.Protocols.Minecraft.ProtocolTypes.MinecraftVersion>(originalText);
            var detailsReserialized = JsonConvert.SerializeObject(details, Formatting.Indented);
            var origJsonRaw = JToken.Parse(originalText);
            var reserializedJsonRaw = JToken.Parse(detailsReserialized);
            Console.WriteLine(detailsReserialized);
            if (!JToken.DeepEquals(origJsonRaw, reserializedJsonRaw)) {
                var tempOutput = new FileInfo(Path.GetTempFileName());
                File.WriteAllText(tempOutput.FullName, detailsReserialized);
                TestResources.LaunchOutputComparison(testMinecraftDetailsFile, tempOutput);
                Assert.True(false);
            }
        }

        [Fact]
        [Trait("Category", "Serialization")]
        public void Assets_1_7_10() {
            var testMinecraftAssetsFile = TestResources.ResourceDirectory.SubFile("MinecraftAssets_1.7.10.json");
            var originalText = File.ReadAllText(testMinecraftAssetsFile.FullName);
            var assets = JsonConvert.DeserializeObject<MPM.Net.Protocols.Minecraft.ProtocolTypes.AssetCollection>(originalText);
            var assetsReserialized = JsonConvert.SerializeObject(assets, Formatting.Indented);
            var origJsonRaw = JToken.Parse(originalText);
            var reserializedJsonRaw = JToken.Parse(assetsReserialized);
            Console.WriteLine(assetsReserialized);
            if (!JToken.DeepEquals(origJsonRaw, reserializedJsonRaw)) {
                var tempOutput = new FileInfo(Path.GetTempFileName());
                File.WriteAllText(tempOutput.FullName, assetsReserialized);
                TestResources.LaunchOutputComparison(testMinecraftAssetsFile, tempOutput);
                Assert.True(false);
            }
        }

        //TODO: Add a unit test checking for download field processing
    }
}
