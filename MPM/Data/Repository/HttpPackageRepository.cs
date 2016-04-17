using System;
using System.Collections.Generic;
using System.Net;
using MPM.Types;
using Newtonsoft.Json;

namespace MPM.Data.Repository {
    public class HttpPackageRepository : IPackageRepository {
        private readonly Uri baseUri;

        public HttpPackageRepository(Uri baseUri) {
            this.baseUri = baseUri;
        }

        public IEnumerable<Build> FetchBuilds() {
            string responseString;
            var fetchTime = Util.TimerUtil.Time(out responseString, () => {
                using (var wc = new WebClient()) {
                    Console.Write($"Full-Fetching builds from {baseUri}...");
                    responseString = wc.DownloadString(new Uri(baseUri, $"/builds/?format=json"));
                }
            });
            Console.WriteLine($" Done in {fetchTime.TotalMilliseconds}ms.");
            return JsonConvert.DeserializeObject<IEnumerable<Build>>(responseString);
        }

        public IEnumerable<Build> FetchBuilds(DateTime updatedAfter) {
            string responseString;
            var fetchTime = Util.TimerUtil.Time(out responseString, () => {
                Console.Write($"Delta-Fetching builds from {this.baseUri}...");
                var updatedAfterString = updatedAfter.ToUniversalTime().ToUnixTimeStamp();
                var uri = new Uri(this.baseUri, $"/builds/?format=json&updatedAfter={updatedAfterString}");
                using (var wc = new WebClient()) {
                    responseString = wc.DownloadString(uri);
                }
            });
            Console.WriteLine($" Done in {fetchTime.TotalMilliseconds}ms.");
            return JsonConvert.DeserializeObject<IEnumerable<Build>>(responseString);
        }
    }
}
