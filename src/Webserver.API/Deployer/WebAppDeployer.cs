// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Extensions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.RequestHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Deployer
{
    /// <summary>
    /// Class used for the automated deployment of WebApps to a PLC (1500)
    /// </summary>
    public class WebAppDeployer
    {
        private readonly IApiRequestHandler ApiRequestHandler;

        /// <summary>
        /// You can implement the functions on your own - the Handler will call the functions of the implementation
        /// that implements the functions of the IApiRequestHandler.
        /// </summary>
        /// <param name="requestHandler"></param>
        public WebAppDeployer(IApiRequestHandler requestHandler)
        {
            this.ApiRequestHandler = requestHandler;
        }

        /// <summary>
        /// make very sure the given webapp contains all the data:
        /// Name
        /// ApplicationResources
        /// PathToWebAppDirectory
        /// </summary>
        /// <param name="webApp"></param>
        /// <returns></returns>
        private void Deploy(ApiWebAppData webApp)
        {
            var res = ApiRequestHandler.WebAppCreate(webApp.Name);
            foreach (var r in webApp.ApplicationResources)
            {
                ApiRequestHandler.DeployResource(webApp, r);
            }
            if (webApp.Not_authorized_page != null)
            {
                 ApiRequestHandler.WebAppSetNotAuthorizedPage(webApp.Name, webApp.Not_authorized_page);
            }

            if (webApp.Not_found_page != null)
            {
                 ApiRequestHandler.WebAppSetNotFoundPage(webApp.Name, webApp.Not_found_page);

            }
            if (webApp.Default_page != null)
            {
                 ApiRequestHandler.WebAppSetDefaultPage(webApp.Name, webApp.Default_page);
            }
        }

        /// <summary>
        /// make very sure the given webapp contains all the data:
        /// Name
        /// ApplicationResources
        /// PathToWebAppDirectory
        /// TAKE CARE: 
        /// If you insert a webapp e.g. that you read from webapp.browse it will not contain the Applicationresources - during delta compare
        /// the function will recognise that there are no resources on the webapp that you want to have on the plc and therefor delete all the resources that are
        /// currently on the plc!
        /// </summary>
        /// <param name="webApp">webappdata - e.g. from parsed webappdirectory</param>
        /// <param name="amountOfTriesForResourceDeployment">optional parameter:
        /// used to determine wether the deployer should retry a upload and compare of the resources found or give up right away (default)
        /// </param>
        /// <returns></returns>
        public void DeployOrUpdate(ApiWebAppData webApp, int amountOfTriesForResourceDeployment = 1)
        {
            var webApps =  ApiRequestHandler.WebAppBrowse();
            if (!webApps.Result.Applications.Any(el => el.Name == webApp.Name))
            {
                 Deploy(webApp);
            }
            else
            {
                // check for changes!
                var browseResourcesResponse =  ApiRequestHandler.WebAppBrowseResources(webApp.Name);
                var browsedResources = browseResourcesResponse.Result.Resources;
                var appOrdered = webApp.ApplicationResources.OrderBy(el => el.Name).ToList();
                List<ApiWebAppResource> browsedOrdered = browsedResources.OrderBy(el => el.Name).ToList();
                // set the IgnoreBOMDifference for all resources that have it set on the app and are found on the plc!
                var appOnBrowsed = browsedOrdered.Where(el => appOrdered.Any(el2 => el.Name == el2.Name)).ToList();
                foreach (var res in appOnBrowsed)
                {
                    var res2 = appOrdered.FirstOrDefault(resource => resource.Name == res.Name) ?? throw new NullReferenceException("resource is not on app but expected to be!!!");
                    res.IgnoreBOMDifference = res2.IgnoreBOMDifference;
                }
                // resources are not all the same data
                // found except those that are on app are those to be deleted
                var browsedExceptApp = browsedOrdered.Except(appOrdered).ToList();
                // app except those that have been found (and equal) are those to be added
                var appExceptBrowsed = appOrdered.Except(browsedOrdered).ToList();
                int tries = 0;
                while (!Enumerable.SequenceEqual(appOrdered, browsedOrdered) && (tries < amountOfTriesForResourceDeployment))
                {
                    foreach (ApiWebAppResource r in browsedExceptApp)
                    {
                         ApiRequestHandler.WebAppDeleteResource(webApp.Name, r.Name);
                    }
                    foreach (ApiWebAppResource r in appExceptBrowsed)
                    {
                        try
                        {
                             ApiRequestHandler.DeployResource(webApp, r);
                        }
                        catch (ApiTicketNotInCompletedStateException)
                        {
                            Console.WriteLine($"Upload for resource: {r.Name} failed for the {(tries + 1).ToString()}. time!");
                        }
                    }
                    browseResourcesResponse =  ApiRequestHandler.WebAppBrowseResources(webApp.Name);
                    browsedResources = browseResourcesResponse.Result.Resources;
                    browsedOrdered = browsedResources.OrderBy(el => el.Name).ToList();
                    browsedExceptApp = browsedOrdered.Except(appOrdered).ToList();
                    appExceptBrowsed = appOrdered.Except(browsedOrdered).ToList();
                    tries++;
                }
                if (!Enumerable.SequenceEqual(appOrdered, browsedOrdered))
                {
                    browsedExceptApp = browsedOrdered.Except(appOrdered).ToList();
                    appExceptBrowsed = appOrdered.Except(browsedOrdered).ToList();
                    var browsedThatShouldntBe = "";
                    var missing = "";
                    browsedExceptApp.ForEach(el => browsedThatShouldntBe = browsedThatShouldntBe + Environment.NewLine + el.Name);
                    appExceptBrowsed.ForEach(el => missing = missing + Environment.NewLine + el.Name);
                    throw new ApiResourceDeploymentFailedException($"Resources found that should were not expected to be on the app:{browsedThatShouldntBe}" +
                        $"Resources that were expected to be on the app but aren't:{missing}");
                }
                var browsedWebAppResp =  ApiRequestHandler.WebAppBrowse(webApp.Name);
                ApiWebAppData browsedWebApp = browsedWebAppResp.Result.Applications.First();
                if (!browsedWebApp.Equals(webApp))
                {
                    // webapp data is not the same
                    if (browsedWebApp.Not_authorized_page != webApp.Not_authorized_page)
                    {
                         ApiRequestHandler.WebAppSetNotAuthorizedPage(webApp.Name, webApp.Not_authorized_page);
                    }
                    if (browsedWebApp.Not_found_page != webApp.Not_found_page)
                    {
                         ApiRequestHandler.WebAppSetNotFoundPage(webApp.Name, webApp.Not_found_page);
                    }
                    if (browsedWebApp.Default_page != webApp.Default_page)
                    {
                         ApiRequestHandler.WebAppSetDefaultPage(webApp.Name, webApp.Default_page);
                    }
                    if(browsedWebApp.State != webApp.State)
                    {
                        ApiRequestHandler.WebAppSetState(webApp.Name, webApp.State);
                    }
                    browsedWebAppResp =  ApiRequestHandler.WebAppBrowse(webApp.Name);
                    browsedWebApp = browsedWebAppResp.Result.Applications.First();
                    if (!browsedWebApp.Equals(webApp))
                    {
                        var serializerSettings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                        throw new ApiWebAppConfigurationFailedException($"browsed WebApp:{Environment.NewLine}" +
                            JsonConvert.SerializeObject(browsedWebApp, serializerSettings)
                            + $"{Environment.NewLine}could not be set to given:{Environment.NewLine}" +
                            JsonConvert.SerializeObject(webApp, serializerSettings));
                    }
                }
            }
        }
    }
}
