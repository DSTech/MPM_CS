using System;
using Alphaleonis.Win32.Filesystem;
using Xunit;
using Newtonsoft.Json;
using MPM.Net.Protocols.Minecraft.ProtocolTypes;
using System.Linq;
using Xunit.Abstractions;

namespace MPMTest.Net.Protocols.Minecraft {
    public class MinecraftVersionDetails {
        private readonly ITestOutputHelper output;

        public MinecraftVersionDetails(ITestOutputHelper output) {
            this.output = output;
        }

        [Fact]
        public void DecipherDetails() {
            var originalText = File.ReadAllText(Path.Combine("TestResources", "MinecraftDetails_1.8.9.json"));
            var details = JsonConvert.DeserializeObject<MPM.Net.Protocols.Minecraft.ProtocolTypes.MinecraftVersion>(
                originalText
            );
            var detailsReserialized = JsonConvert.SerializeObject(details, Formatting.Indented);
            output.WriteLine(detailsReserialized);
        }

        //TODO: Add a unit test checking for download field processing
    }
}
