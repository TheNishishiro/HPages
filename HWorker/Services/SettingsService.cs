using System.IO;
using System.Text.Json.Serialization;
using HWorker.Models;
using Newtonsoft.Json;

namespace HWorker.Services
{
    public static class SettingsService
    {
        public static Settings Load()
        {
            if (!File.Exists("settings.json"))
                return new Settings();
                
            using var sr = new StreamReader("settings.json");
            var content = sr.ReadToEnd();
            if (string.IsNullOrEmpty(content))
                return new Settings();
            return JsonConvert.DeserializeObject<Settings>(content);
        }

        public static void Save(Settings settings)
        {
            var jsonContent = JsonConvert.SerializeObject(settings);

            using var sw = new StreamWriter("settings.json", false);
            sw.WriteLine(jsonContent);
        }
    }
}