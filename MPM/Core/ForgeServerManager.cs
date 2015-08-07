using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MPM.Core {

	public class ForgeServerManager : IServerManager {

		public ForgeServerManager(string directory) {
			this.serverDirectory = directory;
		}

		public readonly string serverDirectory;

		public string Arch {
			get {
				throw new NotImplementedException();
			}
		}

		public string Version {
			get {
				return 1229.ToString();
			}
		}

		public void Update(string version) {
			throw new NotImplementedException();
		}

		public IEnumerable<string> FindUpdates() {
			var results = new List<int>();
			{
				var curVersion = Convert.ToInt32(Version);
				var req = WebRequest.Create("http://files.minecraftforge.net/");
				string text;
				using (var res = req.GetResponse()) {
					using (var textReader = new StreamReader(res.GetResponseStream())) {
						text = textReader.ReadToEnd();
					}
				}
				var regex = new Regex(@"(\d+)\.(\d+)\.(\d+)\.(\d+)");
				var matches = regex.Matches(text);
				var versionsReported = new SortedSet<int>();
				foreach (var match in matches.Cast<Match>()) {
					var groups = match.Groups.Cast<Group>().ToArray();
					var vNum = Convert.ToInt32(groups.Last().Value);
					if (vNum <= curVersion) {
						continue;
					}
					if (versionsReported.Contains(vNum)) {
						continue;
					}
					versionsReported.Add(vNum);
					results.Add(vNum);
				}
			}
			return results.OrderBy(x => x).Select(x => x.ToString());
		}
	}
}
