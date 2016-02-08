using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using JetBrains.Annotations;
using MPM.Extensions;
using MPM.Types;
using Newtonsoft.Json;

namespace MPM.Data.Repository {
    public class HttpPackageRepository : IPackageRepository {
        private readonly Uri baseUri;

        public HttpPackageRepository(Uri baseUri) {
            this.baseUri = baseUri;
        }

        public IEnumerable<Build> FetchBuilds() {
            var req = WebRequest.CreateHttp(new Uri(baseUri, $"/builds/?format=json"));
            byte[] responseData;
            using (var response = req.GetResponse()) {
                responseData = response.GetResponseStream().ReadToEndAndClose();
            }
            return JsonConvert.DeserializeObject<IEnumerable<Build>>(Encoding.UTF8.GetString(responseData));
        }

        public IEnumerable<Build> FetchBuilds(DateTime updatedAfter) {
            var req = WebRequest.CreateHttp(new Uri(baseUri, $"/builds/?format=json&updatedAfter={updatedAfter}"));
            byte[] responseData;
            using (var response = req.GetResponse()) {
                responseData = response.GetResponseStream().ReadToEndAndClose();
            }
            return JsonConvert.DeserializeObject<IEnumerable<Build>>(Encoding.UTF8.GetString(responseData));
        }
    }
}
