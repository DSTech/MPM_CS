using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MPM.Extensions {
    public static class SerialCloning {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.None,
        };

        public static T SerialClone<T>(this T target) => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(target, typeof(T), _settings), _settings);
    }
}
