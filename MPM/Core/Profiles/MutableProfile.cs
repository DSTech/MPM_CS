using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;

namespace MPM.Core.Profiles {
    public static class MutableProfileExtensions {
        public static MutableProfile ToMutableProfile(this IProfile profile) {
            return new MutableProfile {
                Name = profile.Name,
                Preferences = profile.Preferences.ToDictionary(x => x.Key, x => x.Value),
            };
        }
    }

    public class MutableProfile : IProfile {
        public const string YGGDRASIL_ACCESS_TOKEN = "yggdrasilAccessToken";
        public const string YGGDRASIL_PROFILE_ID = "yggdrasilProfileId";
        public const string YGGDRASIL_USER_TYPE = "yggdrasilUserType";

        public MutableProfile() {
        }

        public MutableProfile(String name, IReadOnlyDictionary<string, string> preferences = null) {
            this.Name = name;
            this.Preferences = preferences?.ToDictionary(pair => pair.Key, pair => pair.Value) ?? new Dictionary<string, string>();
        }

        [BsonField]
        public Dictionary<string, string> Preferences { get; set; } = new Dictionary<string, string>();

        [BsonId, BsonField, BsonIndex]
        public string Name { get; set; }

        [BsonIgnore]
        IReadOnlyDictionary<string, string> IProfile.Preferences => Preferences.AsReadOnly();

        [BsonIgnore]
        public string YggdrasilAccessToken {
            get {
                string accessToken;
                if (!Preferences.TryGetValue(YGGDRASIL_ACCESS_TOKEN, out accessToken)) {
                    return null;
                }
                return accessToken;
            }
            set {
                if (value != null) {
                    Preferences[YGGDRASIL_ACCESS_TOKEN] = value;
                } else {
                    Preferences.Remove(YGGDRASIL_ACCESS_TOKEN);
                }
            }
        }

        [BsonIgnore]
        public string YggdrasilProfileId {
            get {
                string profileId;
                if (!Preferences.TryGetValue(YGGDRASIL_PROFILE_ID, out profileId)) {
                    return null;
                }
                return profileId;
            }
            set {
                if (value != null) {
                    Preferences[YGGDRASIL_PROFILE_ID] = value;
                } else {
                    Preferences.Remove(YGGDRASIL_PROFILE_ID);
                }
            }
        }

        [BsonIgnore]
        public string YggdrasilUserType {
            get {
                string userType;
                if (!Preferences.TryGetValue(YGGDRASIL_USER_TYPE, out userType)) {
                    return null;
                }
                return userType;
            }
            set {
                if (value != null) {
                    Preferences[YGGDRASIL_USER_TYPE] = value;
                } else {
                    Preferences.Remove(YGGDRASIL_USER_TYPE);
                }
            }
        }
    }
}
