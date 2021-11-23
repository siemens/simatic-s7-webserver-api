using Newtonsoft.Json;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    public interface IApiWebAppDataSaveSetting
    {
        bool CheckConsistency { get; set; }
        string ConfigurationName { get; set; }
        bool CreateDirectoryIfNotExists { get; set; }
        string DirectoryPath { get; set; }
        JsonSerializerSettings JsonSerializerSetting { get; set; }
    }
}