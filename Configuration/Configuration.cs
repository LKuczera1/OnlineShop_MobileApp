using Microsoft.Maui.Graphics.Text;
using OnlineShop_MobileApp.GUI_elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OnlineShopMobileApp.Configuration
{
    public class ConfigurationProperties
    {
        public class ScreenResolution
        {
            public double x { get; set; }
            public double y { get; set; }
        }

        public class Services
        {
            public String catalog { get; set; }
            public String identity { get; set; }
            public String shopping { get; set; }

            //Todo: deserialization of endpoints to dictionary
            public Dictionary<string, string> CatalogEndpoints { get; set; } = new();
            public Dictionary<string, string> IdentityEndpoints { get; set; } = new();
            public Dictionary<string, string> ShoppingEndpoints { get; set; } = new();
        }

        public string EnvironmentType { get; set; }
        public ScreenResolution screenResolution { get; set; }

        public Services services { get; set; }

        public ConfigurationProperties() 
        {
            screenResolution = new ScreenResolution();
        }
    }


    public class Configuration
    {
        public ConfigurationProperties? Properties { get; set; }


        //Its recommended to use front slash instead of backslash on android
        private const string configurationFileDirectory = "Configuration/Configuration.json";
        public Configuration(bool loadConfigurationNow = true) 
        {
            Properties = new ConfigurationProperties();

            if (loadConfigurationNow)
                LoadConfiguration();

        }

        public void LoadConfiguration()
        {
            using var stream = FileSystem.OpenAppPackageFileAsync(configurationFileDirectory).GetAwaiter().GetResult();
            using var reader = new StreamReader(stream);
            var jsonString = reader.ReadToEnd();

            if (string.IsNullOrWhiteSpace(jsonString))
                throw new Exception("Could not load configuration (empty).");

            using var configurationDoc = JsonDocument.Parse(jsonString);


            if (!configurationDoc.RootElement.TryGetProperty("ConfigurationProperties", out var node))
                throw new Exception("Missing node: ConfigurationProperties");

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                Properties = node.Deserialize<ConfigurationProperties>(options)
                    ?? throw new Exception("Could not deserialize configuration json file");
            }
            catch
            {
                throw;
            }
        }
    }
}
