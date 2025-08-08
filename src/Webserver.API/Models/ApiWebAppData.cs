// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Models
{
    /// <summary>
    /// ApiWebAppData contains information about a plc webapp (to deploy) => State, PageConfiguration => NotFoundPage, NotAuthorizedPage, DefaultPage,...
    /// ApplicationResources: Containing all the resources that are currently on that ApiWebAppData - those whose name matches the protected resources will be protected according to users rights (cookie)
    /// ProtectedResources: List of all resource names that shall be protected 
    /// PathToWebAppDirectory: Path to the Directory containing all the WebAppResources and the WebAppConfiguration File
    /// DirectoriesToIgnoreForUpload: Used e.g. in the ApiWebAppConfigParser to determine if the user wants resources inside that directory to be uploaded (not if directoryname is added)
    /// ResourcesToIgnoreForUpload: Used e.g. in the ApiWebAppConfigParser to determine if the user wants resources to be uploaded (not if resourcename is added)
    /// FileExtensionsToIgnoreForUpload: Used e.g. in the ApiWebAppConfigParser to determine if the user wants resources to be uploaded (not if resource fileextension is added)
    /// </summary>
    public class ApiWebAppData : IApiWebAppData
    {
        /// <summary>
        /// Function to MemberwiseClone an ApiWebAppData to another Object.
        /// </summary>
        /// <returns></returns>
        public ApiWebAppData ShallowCopy()
        {
            return (ApiWebAppData)this.MemberwiseClone();
        }

        /// <summary>
        /// ApiWebApp Name
        /// </summary>
        public string Name { get; set; }

        private ApiWebAppState state;
        /// <summary>
        /// State of ApiWebApp => Enabled/Disabled, None is not valid!
        /// </summary>
        public ApiWebAppState State
        {
            get
            {
                return state;
            }
            set
            {
                if (value == ApiWebAppState.None)
                {
                    throw new ApiInvalidResponseException($"Returned from api was:{value.ToString()} - which is not valid! contact Siemens");
                }
                state = value;
            }
        }

        private ApiWebAppType type;

        /// <summary>
        /// So far (V2.9) only Type "user" and "vot" exists!
        /// </summary>
        public ApiWebAppType Type
        {
            get
            {
                return type;
            }
            set
            {
                if (value == ApiWebAppType.None)
                {
                    throw new ApiInvalidResponseException($"Returned from api was:{value.ToString()} - which is not valid! contact Siemens");
                }
                type = value;
            }
        }

        /// <summary>
        /// The version of the application.
        /// </summary>
        public string Version { get; set; }

        private ApiWebAppRedirectMode redirect_mode;

        /// <summary>
        /// The URL redirect mode of the application.
        /// </summary>
        public ApiWebAppRedirectMode Redirect_mode
        {
            get
            {
                return redirect_mode;
            }
            set
            {
                redirect_mode = value;
            }
        }

        /// <summary>
        /// WebApps DefaultPage: The Page a user should be forwarded to in case he accesses the WebApp (https://plcIPOrDNSName/)~WebAppName or (https://plcIPOrDNSName/)~WebAppName/
        /// If this page is not present the user will be forwarded to the Not_found_page if that one is configured
        /// => e.g. index.html
        /// Wording not c# conform to match Api Responses WebApps
        /// </summary>
        public string Default_page { get; set; }

        /// <summary>
        /// WebApps Not Found Page: The Page a user should be forwarded to in case he does not have provided a resource name that exists on that webapp
        /// => e.g. 404.html
        /// Wording not c# conform to match Api Responses WebApps
        /// </summary>
        public string Not_found_page { get; set; }

        /// <summary>
        /// WebApps Not Authorized Page: The Page a user should be forwarded to in case he does not have the rights to access the requested page (does not have a cookie legitimating him)
        /// => e.g. login.html 
        /// Wording not c# conform to match Api Responses WebApps
        /// </summary>
        public string Not_authorized_page { get; set; }


        /// <summary>
        /// ApplicationResources: Containing all the resources that are currently on that ApiWebAppData - those whose name matches the protected resources will be protected according to users rights (cookie)
        /// </summary>
        [JsonIgnore]
        public List<ApiWebAppResource> ApplicationResources { get; set; }

        /// <summary>
        /// ProtectedResources: List of all resource names that shall be protected 
        /// </summary>
        public List<string> ProtectedResources { get; set; }

        /// <summary>
        /// PathToWebAppDirectory: Path to the Directory containing all the WebAppResources and the WebAppConfiguration File
        /// </summary>
        public string PathToWebAppDirectory { get; set; }

        /// <summary>
        /// DirectoriesToIgnoreForUpload: Used e.g. in the ApiWebAppConfigParser to determine if the user wants resources inside that directory to be uploaded (not if directoryname is added)
        /// </summary>
        public List<string> DirectoriesToIgnoreForUpload;
        /// <summary>
        /// ResourcesToIgnoreForUpload: Used e.g. in the ApiWebAppConfigParser to determine if the user wants resources to be uploaded (not if resourcename is added)
        /// </summary>
        public List<string> ResourcesToIgnoreForUpload;
        /// <summary>
        /// FileExtensionsToIgnoreForUpload: Used e.g. in the ApiWebAppConfigParser to determine if the user wants resources to be uploaded (not if resource fileextension is added)
        /// </summary>
        public List<string> FileExtensionsToIgnoreForUpload;

        /// <summary>
        /// Will compare the webapps: Name, state, Type, Default_page, Not_found_page, Not_authorized_page - and return true if they match!
        /// CARE: Will care for the WebAppConfiguration to match the others webapp Configuration so that a webapp can be detected as the one from e.g. coming back from WebApp.Browse
        /// ProtectedResources,... will not be used in Comparison!
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ApiWebAppData other)
        {
            if (other is null)
                return false;
            var result = this.Name == other.Name && this.State == other.State && this.Type == other.Type && this.Version == other.Version 
                && this.Default_page == other.Default_page && this.Not_found_page == other.Not_found_page 
                && this.Not_authorized_page == other.Not_authorized_page
                    && this.redirect_mode == other.redirect_mode;
            return result;
        }
        /// <summary>
        /// Calls Equals for objects that are ApiWebAppData
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => Equals(obj as ApiWebAppData);
        /// <summary>
        /// GetHashCode => (Name, State, Type, Default_page, Not_found_page, Not_authorized_page).GetHashCode()
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => (Name, State, Type, Version, Redirect_mode, Default_page, Not_found_page, Not_authorized_page).GetHashCode();

        /// <summary>
        /// Return the Json serialized object
        /// </summary>
        /// <returns>Json serialized object</returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
