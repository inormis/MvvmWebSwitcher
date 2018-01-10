using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using WebSwitcher.VSExtensionTools;

namespace WebSwitcher.Configurations {
    public class ConfigurationRepository {
        private const string ConfiguationFileName = "webstwitcher.config";

        public static readonly ConfigurationRepository Instance = new ConfigurationRepository();

        private readonly string _settingsPath;

        private ConfigurationRepository() {
            var solutionDir = Path.GetDirectoryName(ExtensionService.Solution.FullName);
            _settingsPath = Path.Combine(solutionDir, ConfiguationFileName);

            Configuration = File.Exists(_settingsPath)
                ? LoadConfiguration()
                : new Configuration();
        }

        public IConfiguration Configuration { get; }

        private Configuration LoadConfiguration() {
            var content = File.ReadAllText(_settingsPath);
            return JsonConvert.DeserializeObject<Configuration>(content);
        }

        public void Save() {
            var conent = JsonConvert.SerializeObject(Configuration, Formatting.Indented, new StringEnumConverter());
            File.WriteAllText(_settingsPath, conent);
        }
    }
}