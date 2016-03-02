using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Autofac;
using MPM.Core;
using MPM.Core.Profiles;
using MPM.Extensions;
using Newtonsoft.Json;
using PowerArgs;

namespace MPM.CLI {
    public partial class RootArgs {
        [ArgActionMethod]
        [ArgShortcut("auth")]
        public void Login(LoginArgs args) {
            var loginActionProvider = new LoginActionProvider();
            loginActionProvider.Provide(Resolver, args);
        }
    }

    public struct YggdrasilAuthenticationProfile {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("legacy", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public bool Legacy { get; set; }
    }

    public struct YggdrasilAgent {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }
    }

    public struct YggdrasilAuthenticationRequest {
        [JsonProperty("agent")]
        public YggdrasilAgent Agent { get; set; }

        [JsonProperty("username")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("clientToken")]
        public string ClientToken { get; set; }
    }

    public struct YggdrasilAuthenticationResponse {
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }

        [JsonProperty("clientToken")]
        public string ClientToken { get; set; }

        [JsonProperty("availableProfiles")]
        public List<YggdrasilAuthenticationProfile> AvailableProfiles { get; set; }

        [JsonProperty("selectedProfile", Required = Required.Always)]
        public YggdrasilAuthenticationProfile SelectedProfile { get; set; } 
    }

    public class LoginActionProvider : IActionProvider<LoginArgs> {

        public const string YGGDRASIL_CLIENT_TOKEN = "yggdrasilClientToken";

        public static readonly Uri AuthServer = new Uri("https://authserver.mojang.com");
        public void Provide(IContainer resolver, LoginArgs args) {
            var gs = resolver.Resolve<GlobalStorage>();
            var meta = gs.FetchMetaDataManager();

            var clientToken = meta.Get<string>(YGGDRASIL_CLIENT_TOKEN);
            if (clientToken == null) {
                clientToken = Guid.NewGuid().ToString();
            }
            var authData = JsonConvert.SerializeObject(new YggdrasilAuthenticationRequest {
                Agent = new YggdrasilAgent {
                    Name = "Minecraft",
                    Version = 1,
                },
                UserName = args.UserName,
                Password = args.Password,
                ClientToken = clientToken,
            });

            var wreq = WebRequest.CreateHttp(new Uri(AuthServer, "/authenticate"));
            wreq.ContentType = "application/json";
            wreq.Method = "POST";
            var authDataBinary = Encoding.UTF8.GetBytes(authData);
            wreq.ContentLength = authDataBinary.LongLength;
            using (var wreqStream = wreq.GetRequestStream()) {
                wreqStream.Write(authDataBinary);
            }
            YggdrasilAuthenticationResponse authResponse;
            using (var res = (HttpWebResponse)wreq.GetResponse()) {//TODO: Auth error handling (Incorrect credentials or otherwise)
                if (res.StatusCode != HttpStatusCode.OK) {
                    Console.WriteLine("Unexpected response code: {0}:{1}", res.StatusCode, res.StatusDescription);
                    throw new Exception("Failed to authenticate with Yggdrasil");
                }
                using (var resStream = res.GetResponseStream()) {
                    var resData = resStream.ReadToEndAndClose();
                    authResponse = JsonConvert.DeserializeObject<YggdrasilAuthenticationResponse>(Encoding.UTF8.GetString(resData));
                }
            }
            var selectedProfile = authResponse.SelectedProfile;
            var profMan = gs.FetchProfileManager();
            MutableProfile profile;
            var immutableProfile = profMan.Fetch(selectedProfile.Name);
            if (immutableProfile != null) {
                profile = immutableProfile.ToMutableProfile();
            } else {
                profile = new MutableProfile(selectedProfile.Name);
            }

            profile.YggdrasilAccessToken = authResponse.AccessToken;
            profile.YggdrasilProfileId = selectedProfile.Id;
            profile.YggdrasilUserType = selectedProfile.Legacy ? "legacy" : "mojang";

            profMan.Store(profile);

            Console.WriteLine("Authentication information stored for {0} profile {1} ({2})!", immutableProfile == null ? "new" : "existing", selectedProfile.Name, args.UserName);
        }
    }
}
