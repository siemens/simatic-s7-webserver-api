// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// Interface for ApiWebAppData
    /// </summary>
    public interface IApiWebAppData
    {
        /// <summary>
        /// ApplicationResources: Containing all the resources that are currently on that ApiWebAppData - those whose name matches the protected resources will be protected according to users rights (cookie)
        /// </summary>
        List<ApiWebAppResource> ApplicationResources { get; set; }
        /// <summary>
        /// WebApps DefaultPage: The Page a user should be forwarded to in case he accesses the WebApp (https://plcIPOrDNSName/)~WebAppName or (https://plcIPOrDNSName/)~WebAppName/
        /// If this page is not present the user will be forwarded to the Not_found_page if that one is configured
        /// => e.g. index.html
        /// Wording not c# conform to match Api Responses WebApps
        /// </summary>
        string Default_page { get; set; }
        /// <summary>
        /// ApiWebApp Name
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// WebApps Not Authorized Page: The Page a user should be forwarded to in case he does not have the rights to access the requested page (does not have a cookie legitimating him)
        /// => e.g. login.html 
        /// Wording not c# conform to match Api Responses WebApps
        /// </summary>
        string Not_authorized_page { get; set; }
        /// <summary>
        /// WebApps Not Found Page: The Page a user should be forwarded to in case he does not have provided a resource name that exists on that webapp
        /// => e.g. 404.html
        /// Wording not c# conform to match Api Responses WebApps
        /// </summary>
        string Not_found_page { get; set; }
        /// <summary>
        /// PathToWebAppDirectory: Path to the Directory containing all the WebAppResources and the WebAppConfiguration File
        /// </summary>
        string PathToWebAppDirectory { get; set; }
        /// <summary>
        /// ProtectedResources: List of all resource names that shall be protected 
        /// </summary>
        List<string> ProtectedResources { get; set; }
        /// <summary>
        /// State of ApiWebApp => Enabled/Disabled, None is not valid!
        /// </summary>
        ApiWebAppState State { get; set; }
        /// <summary>
        /// So far (V2.9) only Type "user" and "vot" exists!
        /// </summary>
        ApiWebAppType Type { get; set; }
        /// <summary>
        /// Will compare the webapps: Name, state, Type, Default_page, Not_found_page, Not_authorized_page - and return true if they match!
        /// CARE: Will care for the WebAppConfiguration to match the others webapp Configuration so that a webapp can be detected as the one from e.g. coming back from WebApp.Browse
        /// ProtectedResources,... will not be used in Comparison!
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool Equals(ApiWebAppData other);
        /// <summary>
        /// Calls Equals for objects that are ApiWebAppData
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool Equals(object obj);
        /// <summary>
        /// GetHashCode => (Name, State, Type, Default_page, Not_found_page, Not_authorized_page).GetHashCode()
        /// </summary>
        /// <returns></returns>
        int GetHashCode();
        /// <summary>
        /// Function to MemberwiseClone an ApiWebAppData to another Object.
        /// </summary>
        /// <returns></returns>
        ApiWebAppData ShallowCopy();
    }
}