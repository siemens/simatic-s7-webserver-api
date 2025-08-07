// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.WebApp
{
    /// <summary>
    /// Class used for the automated deployment of WebApps to a PLC (1500)
    /// </summary>
    public class ApiWebAppDeployer : IApiWebAppDeployer
    {
        private readonly IApiRequestHandler ApiRequestHandler;
        private readonly IApiResourceHandler ApiResourceHandler;
        private readonly ILogger Logger;

        /// <summary>
        /// Deploys a WebApp to the PLC using the given ApiRequestHandler and ApiResourceHandler.
        /// </summary>
        /// <param name="apiRequestHandler">Api Request Handler (authorized at plc)</param>
        /// <param name="apiResourceHandler">Api Resource Handler - takes care of resource upload, etc.</param>
        /// <param name="logger">Logger</param>
        public ApiWebAppDeployer(IApiRequestHandler apiRequestHandler, IApiResourceHandler apiResourceHandler,
            ILogger logger = null)
        {
            this.ApiRequestHandler = apiRequestHandler;
            this.ApiResourceHandler = apiResourceHandler;
            Logger = logger;
        }

        /// <summary>
        /// make very sure the given webapp contains all the data:
        /// Name
        /// ApplicationResources
        /// PathToWebAppDirectory
        /// </summary>
        /// <param name="webApp"><see cref="ApiWebAppData"/> - e.g. from parsed webappdirectory</param>
        public void Deploy(ApiWebAppData webApp) => DeployAsync(webApp).GetAwaiter().GetResult();
        /// <summary>
        /// make very sure the given webapp contains all the data:
        /// Name
        /// ApplicationResources
        /// PathToWebAppDirectory
        /// </summary>
        /// <param name="webApp"><see cref="ApiWebAppData"/> - e.g. from parsed webappdirectory</param>
        /// <param name="progress">Progress to report to</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        public async Task DeployAsync(ApiWebAppData webApp, IProgress<int> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            Logger?.LogDebug(string.Format("start deploying webapp: {0} with {1} resources.", webApp.Name, webApp.ApplicationResources.Count));
            var res = await ApiRequestHandler.WebAppCreateAsync(webApp, cancellationToken);
            var progressCounter = 0;
            foreach (var r in webApp.ApplicationResources)
            {
                cancellationToken.ThrowIfCancellationRequested();
                await ApiResourceHandler.DeployResourceAsync(webApp, r, cancellationToken);
                progressCounter++;
                progress?.Report(progressCounter * 100 / webApp.ApplicationResources.Count);
            }
            if (webApp.Not_authorized_page != null)
            {
                Logger?.LogDebug($"{nameof(Deploy)}: Set: NotAuthorizedPage.");
                await ApiRequestHandler.WebAppSetNotAuthorizedPageAsync(webApp.Name, webApp.Not_authorized_page, cancellationToken);
            }
            if (webApp.Not_found_page != null)
            {
                Logger?.LogDebug($"{nameof(Deploy)}: Set: NotFoundPage.");
                await ApiRequestHandler.WebAppSetNotFoundPageAsync(webApp.Name, webApp.Not_found_page, cancellationToken);
            }
            if (webApp.Default_page != null)
            {
                Logger?.LogDebug($"{nameof(Deploy)}: Set: DefaultPage.");
                await ApiRequestHandler.WebAppSetDefaultPageAsync(webApp.Name, webApp.Default_page, cancellationToken);
            }
            if (webApp.State != Enums.ApiWebAppState.None)
            {
                Logger?.LogDebug($"{nameof(Deploy)}: set State");
                await ApiRequestHandler.WebAppSetStateAsync(webApp.Name, webApp.State, cancellationToken);
            }
            if (webApp.Redirect_mode != Enums.ApiWebAppRedirectMode.None)
            {
                Logger?.LogDebug($"{nameof(Deploy)}: set UrlRedirectMode");
                await ApiRequestHandler.ApiWebAppSetUrlRedirectModeAsync(webApp.Name, webApp.Redirect_mode, cancellationToken);
            }
            Logger?.LogInformation($"successfully deployed webapp: {webApp.Name} with {webApp.ApplicationResources.Count} resources and set configured pages accordingly.");
        }

        /// <summary>
        /// make very sure the given webapp contains all the data:
        /// Name
        /// ApplicationResources
        /// PathToWebAppDirectory
        /// TAKE CARE: 
        /// If you insert a webapp e.g. that you read from webapp.browse it will not contain the Applicationresources - during delta compare
        /// the function will recognise that there are no resources on the webapp that you want to have on the plc 
        /// and therefor delete all the resources that are
        /// currently on the plc!
        /// </summary>
        /// <param name="webApp"><see cref="ApiWebAppData"/> - e.g. from parsed webappdirectory</param>
        /// <param name="amountOfTriesForResourceDeployment">optional parameter:
        /// used to determine wether the deployer should retry a upload and compare of the resources found or give up right away (default)
        /// </param>
        public void DeployOrUpdate(ApiWebAppData webApp, int amountOfTriesForResourceDeployment = 1) => DeployOrUpdateAsync(webApp, amountOfTriesForResourceDeployment).GetAwaiter().GetResult();

        /// <summary>
        /// make very sure the given webapp contains all the data:
        /// Name
        /// ApplicationResources
        /// PathToWebAppDirectory
        /// TAKE CARE: 
        /// If you insert a webapp e.g. that you read from webapp.browse it will not contain the Applicationresources - during delta compare
        /// the function will recognise that there are no resources on the webapp that you want to have on the plc 
        /// and therefor delete all the resources that are
        /// currently on the plc!
        /// </summary>
        /// <param name="webApp"><see cref="ApiWebAppData"/> - e.g. from parsed webappdirectory</param>
        /// <param name="amountOfTriesForResourceDeployment">optional parameter:
        /// used to determine wether the deployer should retry a upload and compare of the resources found or give up right away (default)
        /// </param>
        /// <param name="progress">Progress to report to</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        public async Task DeployOrUpdateAsync(ApiWebAppData webApp, int amountOfTriesForResourceDeployment = 1, IProgress<int> progress = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var webApps = await ApiRequestHandler.WebAppBrowseAsync(cancellationToken: cancellationToken);
            if (!webApps.Result.Applications.Any(el => el.Name == webApp.Name))
            {
                await DeployAsync(webApp);
            }
            else
            {
                Logger?.LogInformation($"Start {nameof(DeployOrUpdate)} for webApp: {webApp.Name}");
                // check for changes!
                var browseResourcesResponse = await ApiRequestHandler.WebAppBrowseResourcesAsync(webApp, cancellationToken: cancellationToken);
                var browsedResources = browseResourcesResponse.Result.Resources;
                var appOrdered = webApp.ApplicationResources.OrderBy(el => el.Name).ToList();
                var browsedOrdered = browsedResources.OrderBy(el => el.Name).ToList();
                // set the IgnoreBOMDifference for all resources that have it set on the app and are found on the plc!
                var appOnBrowsed = browsedOrdered.Where(el => appOrdered.Any(el2 => el.Name == el2.Name)).ToList();
                foreach (var res in appOnBrowsed)
                {
                    var res2 = appOrdered.FirstOrDefault(resource => resource.Name == res.Name)
                        ?? throw new NullReferenceException("resource is not on app but expected to be!!!");
                    res.IgnoreBOMDifference = res2.IgnoreBOMDifference;
                }
                // found except those that are on app are those to be deleted
                var browsedExceptApp = browsedOrdered.Except(appOrdered).ToList();
                // app except those that have been found (and equal) are those to be added
                var appExceptBrowsed = appOrdered.Except(browsedOrdered).ToList();
                int tries = 0;
                // resources are not all the same data
                while (!Enumerable.SequenceEqual(appOrdered, browsedOrdered) && (tries < amountOfTriesForResourceDeployment))
                {
                    // elements that should further be deleted but are not found in the comparison before!
                    var elemsToFurtherdelete = (browsedOrdered.Where(el => appExceptBrowsed
                    .Any(el2 => el2.Name == el.Name && !(browsedExceptApp.Any(el3 => el3.Name == el2.Name))))).ToList();
                    if (elemsToFurtherdelete.Count != 0)
                    {
                        throw new Exception("Comparison insufficient!");
                    }
                    foreach (ApiWebAppResource r in browsedExceptApp)
                    {
                        Logger?.LogDebug(string.Format("{0}: start deleting: {1} resources.", nameof(DeployOrUpdate), browsedExceptApp.Count));
                        await ApiRequestHandler.WebAppDeleteResourceAsync(webApp.Name, r.Name);
                    }
                    if(browsedExceptApp.Count != 0)
                    {
                        Logger?.LogDebug($"{nameof(DeployOrUpdate)}: done deleting resources.");
                    }
                    var progressCounter = 0;
                    foreach (ApiWebAppResource r in appExceptBrowsed)
                    {
                        Logger?.LogDebug(string.Format("{0}: start uploading: {1} resources.", nameof(DeployOrUpdate), appExceptBrowsed.Count));
                        cancellationToken.ThrowIfCancellationRequested();
                        try
                        {
                            await ApiResourceHandler.DeployResourceAsync(webApp, r);
                            progressCounter++;
                            progress?.Report(progressCounter * 100 / appExceptBrowsed.Count);
                        }
                        catch (ApiTicketNotInCompletedStateException e)
                        {
                            Logger?.LogWarning(e, $"Upload for resource: {r.Name} failed for the {(tries + 1).ToString()}. time!");
                        }
                    }
                    if (appExceptBrowsed.Count != 0)
                    {
                        Logger?.LogDebug($"{nameof(DeployOrUpdate)}: done uploading resources.");
                    }
                    browseResourcesResponse = await ApiRequestHandler.WebAppBrowseResourcesAsync(webApp, cancellationToken: cancellationToken);
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
                var browsedWebAppResp = await ApiRequestHandler.WebAppBrowseAsync(webApp, cancellationToken);
                ApiWebAppData browsedWebApp = browsedWebAppResp.Result.Applications.First();
                if (!browsedWebApp.Equals(webApp))
                {
                    // webapp data is not the same
                    if (browsedWebApp.Not_authorized_page != webApp.Not_authorized_page)
                    {
                        Logger?.LogDebug($"{nameof(DeployOrUpdate)}: set NotAuthorizedPage");
                        await ApiRequestHandler.WebAppSetNotAuthorizedPageAsync(webApp.Name, webApp.Not_authorized_page, cancellationToken);
                    }
                    if (browsedWebApp.Not_found_page != webApp.Not_found_page)
                    {
                        Logger?.LogDebug($"{nameof(DeployOrUpdate)}: set NotFoundPage");
                        await ApiRequestHandler.WebAppSetNotFoundPageAsync(webApp.Name, webApp.Not_found_page, cancellationToken);
                    }
                    if (browsedWebApp.Default_page != webApp.Default_page)
                    {
                        Logger?.LogDebug($"{nameof(DeployOrUpdate)}: set DefaultPage");
                        await ApiRequestHandler.WebAppSetDefaultPageAsync(webApp.Name, webApp.Default_page, cancellationToken);
                    }
                    if (browsedWebApp.State != webApp.State)
                    {
                        Logger?.LogDebug($"{nameof(DeployOrUpdate)}: set State");
                        await ApiRequestHandler.WebAppSetStateAsync(webApp.Name, webApp.State, cancellationToken);
                    }
                    if (browsedWebApp.Redirect_mode != webApp.Redirect_mode)
                    {
                        Logger?.LogDebug($"{nameof(DeployOrUpdate)}: set RedirectMode");
                        if (webApp.Redirect_mode == Enums.ApiWebAppRedirectMode.None)
                        {
                            throw new ApiInvalidParametersException("Redirect mode should never be none!");
                        }
                        await ApiRequestHandler.ApiWebAppSetUrlRedirectModeAsync(webApp.Name, webApp.Redirect_mode, cancellationToken);
                    }
                    browsedWebAppResp = await ApiRequestHandler.WebAppBrowseAsync(webApp, cancellationToken);
                    browsedWebApp = browsedWebAppResp.Result.Applications.First();
                    if (!browsedWebApp.Equals(webApp))
                    {
                        var serializerSettings = new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() };
                        throw new ApiWebAppConfigurationFailedException($"browsed WebApp:{Environment.NewLine}" +
                            JsonConvert.SerializeObject(browsedWebApp, serializerSettings)
                            + $"{Environment.NewLine}could not be set to given:{Environment.NewLine}" +
                            JsonConvert.SerializeObject(webApp, serializerSettings));
                    }
                    Logger?.LogInformation($"successfully updated webapp: {webApp.Name}, its resources and set configured pages accordingly.");
                }
            }
        }
    }
}
