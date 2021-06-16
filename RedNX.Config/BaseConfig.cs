using System.Text.Json.Serialization;

namespace RedNX.Config {
    public abstract class BaseConfig {

        [JsonPropertyName("version")]
        public int Version { get; set; }

        protected BaseConfig(int version) {
            Version = version;
        }

    }
}
