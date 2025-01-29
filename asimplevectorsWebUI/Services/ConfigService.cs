using System.Text.Json;

namespace asimplevectorsWebUI.Services
{
    public class ConfigService
    {
        private const string ConfigFilePath = "/data/config.json";
        public string ServerUrl { get; private set; } = string.Empty;

        public ConfigService()
        {
            LoadConfig();
        }

        public void LoadConfig()
        {
            if (File.Exists(ConfigFilePath))
            {
                var configJson = File.ReadAllText(ConfigFilePath);
                var config = JsonSerializer.Deserialize<Config>(configJson);
                ServerUrl = config?.ServerUrl ?? string.Empty;
            }
        }

        public void SaveConfig(string serverUrl)
        {
            var config = new Config { ServerUrl = serverUrl };
            var configJson = JsonSerializer.Serialize(config);
            File.WriteAllText(ConfigFilePath, configJson);
            ServerUrl = serverUrl;
        }

        private class Config
        {
            public string ServerUrl { get; set; } = string.Empty;
        }
    }
}