using System.Collections.Generic;
using Siemens.Simatic.S7.Webserver.API.Enums;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    public interface IApiWebAppData
    {
        List<ApiWebAppResource> ApplicationResources { get; set; }
        string Default_page { get; set; }
        string Name { get; set; }
        string Not_authorized_page { get; set; }
        string Not_found_page { get; set; }
        string PathToWebAppDirectory { get; set; }
        List<string> ProtectedResources { get; set; }
        ApiWebAppState State { get; set; }
        ApiWebAppType Type { get; set; }

        bool Equals(ApiWebAppData other);
        bool Equals(object obj);
        int GetHashCode();
        ApiWebAppData ShallowCopy();
    }
}