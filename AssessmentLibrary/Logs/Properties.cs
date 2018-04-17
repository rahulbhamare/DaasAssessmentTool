using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssessmentLibrary.Logs
{
    public class Properties
    {
        [JsonProperty("debug.load-providers-from-default-folder")]
        public bool DebugLoadProvidersFromDefaultFolder { get; set; } = false;

        [JsonProperty("debug.transfer-data")]
        public bool DebugTransferData { get; set; } = false;

        [JsonProperty("debug.encrypt-state-and-history")]
        public bool DebugEncryptStateAndHistory { get; set; } = false;

        [JsonProperty("debug.api.run-client-from-visual-studio-debug")]
        public bool DebugApiRunFromVisualStudioDebug { get; set; } = false;

        [JsonProperty("provider.black-list")]
        public List<string> ProviderBlacklist { get; set; } = new List<string>();

        [JsonProperty("provider.white-list")]
        public List<string> ProviderWhitelist { get; set; } = new List<string>();

        [JsonProperty("provider.frequency-filtering-enabled")]
        public bool ProviderFrequencyFilteringEnabled { get; set; } = true;

        [JsonProperty("provider.state-refresh-enabled")]
        public bool ProviderRefreshEnabled { get; set; } = true;

        [JsonProperty("json-retention")]
        public bool JsonRetention { get; set; } = false;

        [JsonProperty("configuration-update-enabled")]
        public bool ConfigurationUpdateEnabled { get; set; } = true;

        [JsonProperty("test-mode")]
        public bool TestMode { get; set; } = false;

        [JsonProperty("test.max-concurrent-threads")]
        public int? MaxConcurrentThreads { get; set; } = null;

        [JsonProperty("test.max-json-size-bytes")]
        public int? MaxJsonSize { get; set; } = null;

        [JsonProperty("test.hash-bucket")]
        public int? HashBucket { get; set; } = null;
        
        public DateTime? OverrideFilteringDay { get; set; } = null;
        
        [JsonProperty("test.decrypted-config")]
        public bool AlreadyDecrypted { get; set; } = false;

        [JsonProperty("test.encrypt-monitor-data")]
        public bool EncryptMonitorData { get; set; } = true;

        [JsonProperty("test.config-file-enabled")]
        public bool TestConfigFileEnabled { get; set; } = false;

        [JsonProperty("test.config-file-path")]
        public string TestConfigFilePath { get; set; } = null;
    }
}
