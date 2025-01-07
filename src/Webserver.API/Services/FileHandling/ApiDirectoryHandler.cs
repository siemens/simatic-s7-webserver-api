// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.FileHandling
{
    /// <summary>
    /// Handler for Directory Down/Upload/Update
    /// </summary>
    public class ApiDirectoryHandler : IApiDirectoryHandler
    {
        private readonly IApiRequestHandler ApiRequestHandler;
        private readonly IApiFileHandler ApiFileHandler;

        /// <summary>
        /// Handler for Directory Down/Upload/Update
        /// </summary>
        /// <param name="apiRequestHandler">Request handler to send the api requests with</param>
        /// <param name="apiFileHandler">Request handler to take care of file up/download</param>
        public ApiDirectoryHandler(IApiRequestHandler apiRequestHandler, IApiFileHandler apiFileHandler)
        {
            ApiRequestHandler = apiRequestHandler;
            ApiFileHandler = apiFileHandler;
        }

        /// <summary>
        /// make very sure the given resource contains all the data:
        /// Resources
        /// TAKE CARE: 
        /// If you insert a resource which does not contain all the Applicationresources with SubNodes 
        /// the function will only upload the resource and its direct sub-resources
        /// </summary>
        /// <param name="resource"><see cref="ApiFileResource"/> - e.g. from parsed directory</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        public async Task DeployAsync(ApiFileResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resource.Type == Enums.ApiFileResourceType.File)
            {
                await ApiFileHandler.DeployFileAsync(resource, cancellationToken);
            }
            else
            {
                var dirName = resource.GetVarNameForMethods();
                await ApiRequestHandler.FilesCreateDirectoryAsync(dirName, cancellationToken);
                foreach (var subres in resource.Resources)
                {
                    await DeployAsync(subres);
                }
            }
        }
        /// <summary>
        /// make very sure the given resource contains all the data:
        /// Resources
        /// TAKE CARE: 
        /// If you insert a resource which does not contain all the Applicationresources with SubNodes 
        /// the function will only upload the resource and its direct sub-resources
        /// </summary>
        /// <param name="resource"><see cref="ApiFileResource"/> - e.g. from parsed directory</param>
        public void Deploy(ApiFileResource resource) => DeployAsync(resource).GetAwaiter().GetResult();

        /// <summary>
        /// Delete the given resource (and all its sub-resources)
        /// </summary>
        /// <param name="resource">the resource to delete</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Task for deletion</returns>
        public async Task DeleteAsync(ApiFileResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resource.Type == Enums.ApiFileResourceType.File)
            {
                var resName = resource.GetVarNameForMethods();
                try
                {
                    await ApiRequestHandler.FilesDeleteAsync(resName, cancellationToken);
                }
                catch (ApiEntityDoesNotExistException) { }
            }
            else
            {
                if (resource.Resources != null)
                {
                    foreach (var file in resource.Resources)
                    {
                        await DeleteAsync(file, cancellationToken);
                    }
                }
                else
                {
                    Console.WriteLine("!!!");
                }
                var dirName = resource.GetVarNameForMethods();
                try
                {
                    await ApiRequestHandler.FilesDeleteDirectoryAsync(dirName, cancellationToken);
                }
                catch (ApiEntityDoesNotExistException) { }

            }
        }

        /// <summary>
        /// Delete the given resource (and all its sub-resources)
        /// </summary>
        /// <param name="resource">the resource to delete</param>
        public void Delete(ApiFileResource resource)
            => DeleteAsync(resource).GetAwaiter().GetResult();

        /// <summary>
        /// Get all Resources underneath the given resource
        /// </summary>
        /// <param name="resource">resouce to be browsed</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>A resource containing everything that is present underneath</returns>
        public async Task<ApiFileResource> BrowseAndBuildResourceAsync(ApiFileResource resource, CancellationToken cancellationToken = default(CancellationToken))
        {
            // should we clone here?
            var res = (ApiFileResource)resource.Clone();
            res.Resources = new List<ApiFileResource>();
            var browseResponse = await ApiRequestHandler.FilesBrowseAsync(resource.GetVarNameForMethods(), cancellationToken);
            if (resource.Type == Enums.ApiFileResourceType.Dir)
            {
                res.Resources = browseResponse.Result.Resources;
                var parentsForChildren = new List<ApiFileResource>();
                foreach (var parent in res.Parents)
                {
                    parentsForChildren.Add(parent);
                }
                parentsForChildren.Add(res);
                var toSet = new List<ApiFileResource>();
                foreach (var subRes in res.Resources)
                {
                    subRes.Parents = parentsForChildren;
                    var toAdd = await BrowseAndBuildResourceAsync(subRes, cancellationToken);
                    toAdd.Parents = parentsForChildren;
                    toSet.Add(toAdd);
                }
                res.Resources = toSet;
            }
            res.Resources = res.Resources.OrderBy(el => el.Type).ThenBy(el => el.Size).ToList();
            return res;
        }

        /// <summary>
        /// Get all Resources underneath the given resource
        /// </summary>
        /// <param name="resource">resouce to be browsed</param>
        /// <returns>A resource containing everything that is present underneath</returns>
        public ApiFileResource BrowseAndBuildResource(ApiFileResource resource)
            => BrowseAndBuildResourceAsync(resource).GetAwaiter().GetResult();

        /// <summary>
        /// Update the given Resource - and SubResources, when necessary
        /// </summary>
        /// <param name="resource">the resource to be updated</param>
        /// <param name="browsedResource">the resource returned by browsing the plc - make sure the sub-Nodes are present (!)</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Task to update the resource</returns>
        public async Task UpdateResourceAsync(ApiFileResource resource, ApiFileResource browsedResource, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (browsedResource.Type != resource.Type)
            {
                await DeleteAsync(browsedResource, cancellationToken);
                await DeployAsync(resource, cancellationToken);
            }
            if (resource.Type == Enums.ApiFileResourceType.File)
            {
                await UpdateFileResourceAsync(resource, browsedResource, cancellationToken);
            }
            else
            {
                foreach (var subResource in resource.Resources)
                {
                    var match = browsedResource.Resources.FirstOrDefault(el => el.Name == subResource.Name);
                    if (match == null)
                    {
                        await DeployAsync(subResource);
                    }
                    else
                    {
                        var browseResult = await ApiRequestHandler.FilesBrowseAsync(subResource.GetVarNameForMethods(), cancellationToken);
                        match.Resources = browseResult.Result.Resources;
                        await UpdateResourceAsync(subResource, match, cancellationToken);
                    }
                }
            }
        }

        /// <summary>
        /// Update the given Resource - and SubResources, when necessary
        /// </summary>
        /// <param name="resource">the resource to be updated</param>
        /// <param name="browsedResource">the resource returned by browsing the plc - make sure the sub-Nodes are present (!)</param>
        public void UpdateResource(ApiFileResource resource, ApiFileResource browsedResource)
            => UpdateResourceAsync(resource, browsedResource).GetAwaiter().GetResult();

        /// <summary>
        /// Update the given File Resource when necessary
        /// </summary>
        /// <param name="resource">the file to be updated</param>
        /// <param name="browsedResource">the file returned by browsing the plc</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Task to update the File</returns>
        public async Task UpdateFileResourceAsync(ApiFileResource resource, ApiFileResource browsedResource, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (resource.Type != Enums.ApiFileResourceType.File)
            {
                throw new ArgumentException($"{nameof(resource)} is not of type file! instead: {resource.Type}");
            }
            if (browsedResource.Type != Enums.ApiFileResourceType.File)
            {
                throw new ArgumentException($"{nameof(browsedResource)} is not of type file! instead: {browsedResource.Type}");
            }
            // compare if an update is necessary
            if (!resource.Equals(browsedResource))
            {
                await ApiRequestHandler.FilesDeleteAsync(resource.GetVarNameForMethods(), cancellationToken);
                await ApiFileHandler.DeployFileAsync(resource, cancellationToken);
            }
        }

        /// <summary>
        /// Update the given File Resource when necessary
        /// </summary>
        /// <param name="resource">the file to be updated</param>
        /// <param name="browsedResource">the file returned by browsing the plc</param>
        public void UpdateFileResource(ApiFileResource resource, ApiFileResource browsedResource)
            => UpdateFileResourceAsync(resource, browsedResource).GetAwaiter().GetResult();

        /// <summary>
        /// make very sure the given resource contains all the data:
        /// Resources
        /// TAKE CARE: 
        /// If you insert a resource e.g. that you read from files.browse it will not contain all the Applicationresources with SubNodes - during delta compare
        /// the function will recognise that there are no further resources on the directory that you want to have on the plc 
        /// and therefor delete all the other sub-resources that are
        /// currently on the plc!
        /// </summary>
        /// <param name="resource"><see cref="ApiFileResource"/> - e.g. from parsed webappdirectory</param>
        /// <param name="amountOfTriesForResourceDeployment">optional parameter:
        /// used to determine wether the DirectoryHandler should retry a upload and compare of the resources found or give up right away (default)
        /// </param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        public async Task DeployOrUpdateAsync(ApiFileResource resource, int amountOfTriesForResourceDeployment = 1, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (amountOfTriesForResourceDeployment < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(amountOfTriesForResourceDeployment));
            }
            try
            {
                var browsedResource = await BrowseAndBuildResourceAsync(resource, cancellationToken);
                int tries = 0;
                while (!browsedResource.Equals(resource) && tries < amountOfTriesForResourceDeployment)
                {
                    await UpdateResourceAsync(resource, browsedResource, cancellationToken);
                    tries++;
                    // make sure the browsedResource now is Equal to the one we have - for a directory this means all subNodes are also equal, they are not browsed during the "standard" BRowse
                    browsedResource = await BrowseAndBuildResourceAsync(resource, cancellationToken);
                    resource.Resources = resource.Resources.OrderBy(el => el.Type).ThenBy(el => el.Size).ToList();
                    browsedResource.Resources = browsedResource.Resources.OrderBy(el => el.Type).ThenBy(el => el.Size).ToList();
                }
                if (!browsedResource.Equals(resource))
                {
                    StringBuilder errorMessage = new StringBuilder($"The browsed resource still does not Equal the given resource! tries: {tries}");
                    try
                    {
                        errorMessage.AppendLine(JsonConvert.SerializeObject(browsedResource));
                    }
                    catch (Exception e)
                    {
                        errorMessage.AppendLine(e.ToString());
                    }
                    try
                    {
                        errorMessage.AppendLine(JsonConvert.SerializeObject(resource));
                    }
                    catch (Exception e)
                    {
                        errorMessage.AppendLine(e.ToString());
                    }
                    throw new InvalidOperationException(errorMessage.ToString());
                }
            }
            catch (ApiEntityDoesNotExistException)
            {
                await DeployAsync(resource, cancellationToken);
            }
        }
        /// <summary>
        /// make very sure the given resource contains all the data:
        /// Resources
        /// TAKE CARE: 
        /// If you insert a resource e.g. that you read from files.browse it will not contain all the Applicationresources with SubNodes - during delta compare
        /// the function will recognise that there are no further resources on the directory that you want to have on the plc 
        /// and therefor delete all the other sub-resources that are
        /// currently on the plc!
        /// </summary>
        /// <param name="resource"><see cref="ApiFileResource"/> - e.g. from parsed webappdirectory</param>
        /// <param name="amountOfTriesForResourceDeployment">optional parameter:
        /// used to determine wether the DirectoryHandler should retry a upload and compare of the resources found or give up right away (default)
        /// </param>
        public void DeployOrUpdate(ApiFileResource resource, int amountOfTriesForResourceDeployment = 1)
         => DeployOrUpdateAsync(resource, amountOfTriesForResourceDeployment).GetAwaiter().GetResult();
    }
}
