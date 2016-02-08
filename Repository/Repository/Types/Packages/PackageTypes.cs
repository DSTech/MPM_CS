using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MPM.Types;
using Newtonsoft.Json;
using NServiceKit;
using NServiceKit.ServiceHost;
using NServiceKit.ServiceInterface;
using NServiceKit.Text;

namespace Repository.Types.Packages {
    static class JsonSetup {
        static JsonSetup() {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Auto,
            };
        }

        public static void Setup() {
            if (_isSetup) {
                return;
            }
            JsConfig<Build>.RawSerializeFn = _serializeBuild;
            JsConfig<Build>.RawDeserializeFn= _deserializeBuild;
            _isSetup = true;
        }

        private static string _serializeBuild(Build build) {
            return JsonConvert.SerializeObject(build, typeof(Build), Formatting.Indented, JsonConvert.DefaultSettings());
        }

        private static Build _deserializeBuild(string buildStr) {
            return JsonConvert.DeserializeObject<Build>(buildStr, JsonConvert.DefaultSettings());
        }

        private static bool _isSetup = false;
    }

    [Route("/builds", "GET")]
    public class BuildListRequest : IReturn<List<Build>> {
        static BuildListRequest() {
            JsonSetup.Setup();
        }
        [DataMember(Name = "updatedAfter", IsRequired = false)]
        public DateTime? UpdatedAfter { get; set; }
    }

    [Route("/builds", "POST")]
    public class BuildSubmission : IReturn<Build> {
        static BuildSubmission() {
            JsonSetup.Setup();
        }
    }
    
    [Route("/builds", "DELETE")]
    public class BuildDeletionRequest : IReturn<Build> {
        static BuildDeletionRequest() {
            JsonSetup.Setup();
        }
    }
}
