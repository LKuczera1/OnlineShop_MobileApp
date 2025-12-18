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

        public class ConnectionProperties
        {
            public String catalog { get; set; }
            public String identity { get; set; }
            public String shopping { get; set; }
        }

        public string EnvironmentType { get; set; }
        public ScreenResolution screenResolution { get; set; }
        public bool DarkTheme { get; set; }

        public ConnectionProperties connectionProperties { get; set; }

        public ConfigurationProperties() 
        {
            screenResolution = new ScreenResolution();
        }
    }


    public class Configuration
    {
        public ConfigurationProperties? Properties { get; set; }

        private string tempConfigDirectory = "D:\\Programming\\Projects\\Visual Studio\\OnlineShop_MobileApp\\Configuration\\Configuration.json";
        public Configuration() 
        {
            Properties = new ConfigurationProperties();

            string configPath = Path.Combine(
            FileSystem.AppDataDirectory,
            "config.json"
);

        }

        public void LoadConfiguration()
        {
            string jsonString;

            try
            {
                jsonString = File.ReadAllText(tempConfigDirectory);
            }
            catch(Exception e)
            {
                return;
            }
            

            var serializerOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
            };

            if (jsonString==null)
                throw new Exception("Could not load configuration.");

            try
            {
                var doc = JsonDocument.Parse(jsonString);

                Properties = doc.RootElement.GetProperty("ConfigurationProperties")
                    .Deserialize<ConfigurationProperties>(serializerOptions);
            }
            catch (Exception exception)
            {
                throw new Exception("Could not load configuration: " + exception.ToString());
            }
        }
    }
}
