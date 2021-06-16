using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedNX.Config {
    public static class ConfigManager {

        private static readonly JsonSerializerOptions NormalOptions = new JsonSerializerOptions { WriteIndented = true };
        private static readonly JsonSerializerOptions IndentedOptions = new JsonSerializerOptions { WriteIndented = true };

        #region Save

        public static bool SaveToFile<T>(T config, string path, bool indented = true) where T : BaseConfig {
            try {
                using var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                using var sw = new StreamWriter(fs);
                sw.Write(JsonSerializer.Serialize(config, indented ? IndentedOptions : NormalOptions));
                sw.Flush();
                fs.Flush();
                sw.Close();
                fs.Close();
                return true;
            } catch {
                return false;
            }
        }

        public static async Task<bool> SaveToFileAsync<T>(T config, string path, bool indented = true) where T : BaseConfig {
            try {
                await using var fs = new FileStream(path, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
                await JsonSerializer.SerializeAsync(fs, config, indented ? IndentedOptions : NormalOptions);
                await fs.FlushAsync();
                fs.Close();
                return true;
            } catch {
                return false;
            }
        }

        public static bool SaveToString<T>(T config, out string json, bool indented = true) where T : BaseConfig {
            try {
                json = JsonSerializer.Serialize(config, indented ? IndentedOptions : NormalOptions);
                return true;
            } catch {
                json = null;
                return false;
            }
        }

        #endregion

        #region Load

        public static bool LoadFromFile<T>(string path, out T data) {
            try {
                using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                using var sr = new StreamReader(fs);
                string json = sr.ReadToEnd();
                sr.Close();
                fs.Close();
                data = JsonSerializer.Deserialize<T>(json);
                return true;
            } catch {
                data = default(T);
                return false;
            }
        }

        public static async Task<(bool, T)> LoadWithCheckFromFileAsync<T>(string path) {
            try {
                await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var data = await JsonSerializer.DeserializeAsync<T>(fs);
                fs.Close();
                return (true, data);
            } catch {
                return (false, default(T));
            }
        }

        public static async Task<T> LoadFromFileAsync<T>(string path) {
            try {
                await using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var data = await JsonSerializer.DeserializeAsync<T>(fs);
                fs.Close();
                return data;
            } catch {
                return default(T);
            }
        }

        public static bool LoadFromString<T>(string json, out T data) {
            try {
                data = JsonSerializer.Deserialize<T>(json);
                return true;
            } catch {
                data = default(T);
                return false;
            }
        }

        #endregion
    }
}
