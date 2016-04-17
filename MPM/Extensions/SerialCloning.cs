namespace Newtonsoft.Json {
    public static class SerialCloning {
        private static readonly JsonSerializerSettings _settings = new JsonSerializerSettings {
            TypeNameHandling = TypeNameHandling.Auto,
            Formatting = Formatting.None,
        };

        public static T SerialClone<T>(this T target) => JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(target, typeof(T), _settings), _settings);
    }
}
