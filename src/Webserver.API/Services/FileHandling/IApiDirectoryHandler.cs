// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.FileHandling
{
    /// <summary>
    /// Handler for Directory Down/Upload/Update
    /// </summary>
    public interface IApiDirectoryHandler
    {
        /// <summary>
        /// Get all Resources underneath the given resource
        /// </summary>
        /// <param name="resource">resouce to be browsed</param>
        /// <returns>A resource containing everything that is present underneath</returns>
        ApiFileResource BrowseAndBuildResource(ApiFileResource resource);
        /// <summary>
        /// Get all Resources underneath the given resource
        /// </summary>
        /// <param name="resource">resouce to be browsed</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>A resource containing everything that is present underneath</returns>
        Task<ApiFileResource> BrowseAndBuildResourceAsync(ApiFileResource resource, CancellationToken cancellationToken = default);
        /// <summary>
        /// Delete the given resource (and all its sub-resources)
        /// </summary>
        /// <param name="resource">the resource to delete</param>
        void Delete(ApiFileResource resource);
        /// <summary>
        /// Delete the given resource (and all its sub-resources)
        /// </summary>
        /// <param name="resource">the resource to delete</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <param name="progress">Progress to report to.</param>
        /// <returns>Task for deletion</returns>
        Task DeleteAsync(ApiFileResource resource, CancellationToken cancellationToken = default, IProgress<int> progress = null);
        /// <summary>
        /// make very sure the given resource contains all the data:
        /// Resources
        /// TAKE CARE: 
        /// If you insert a resource which does not contain all the Applicationresources with SubNodes 
        /// the function will only upload the resource and its direct sub-resources
        /// </summary>
        /// <param name="resource"><see cref="ApiFileResource"/> - e.g. from parsed directory</param>
        void Deploy(ApiFileResource resource);
        /// <summary>
        /// make very sure the given resource contains all the data:
        /// Resources
        /// TAKE CARE: 
        /// If you insert a resource which does not contain all the Applicationresources with SubNodes 
        /// the function will only upload the resource and its direct sub-resources
        /// </summary>
        /// <param name="resource"><see cref="ApiFileResource"/> - e.g. from parsed directory</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <param name="progress">Progress to report to.</param>
        Task DeployAsync(ApiFileResource resource, CancellationToken cancellationToken = default, IProgress<int> progress = null);
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
        /// used to determine whether the DirectoryHandler should retry a upload and compare of the resources found or give up right away (default)
        /// </param>
        void DeployOrUpdate(ApiFileResource resource, int amountOfTriesForResourceDeployment = 1);
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
        /// used to determine whether the DirectoryHandler should retry a upload and compare of the resources found or give up right away (default)
        /// </param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <param name="progress">Progress to report to.</param>
        Task DeployOrUpdateAsync(ApiFileResource resource, int amountOfTriesForResourceDeployment = 1, CancellationToken cancellationToken = default,
            IProgress<int> progress = null);
        /// <summary>
        /// Update the given File Resource when necessary
        /// </summary>
        /// <param name="resource">the file to be updated</param>
        /// <param name="browsedResource">the file returned by browsing the plc</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>Task to update the File</returns>
        Task UpdateFileResourceAsync(ApiFileResource resource, ApiFileResource browsedResource, CancellationToken cancellationToken = default);
        /// <summary>
        /// Update the given File Resource when necessary
        /// </summary>
        /// <param name="resource">the file to be updated</param>
        /// <param name="browsedResource">the file returned by browsing the plc</param>
        void UpdateFileResource(ApiFileResource resource, ApiFileResource browsedResource);

        /// <summary>
        /// Update the given Resource - and SubResources, when necessary
        /// </summary>
        /// <param name="resource">the resource to be updated</param>
        /// <param name="browsedResource">the resource returned by browsing the plc - make sure the sub-Nodes are present (!)</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <param name="progress">Progress to report to.</param>
        /// <returns>Task to update the resource</returns>
        Task UpdateResourceAsync(ApiFileResource resource, ApiFileResource browsedResource, CancellationToken cancellationToken = default, IProgress<int> progress = null);
        /// <summary>
        /// Update the given Resource - and SubResources, when necessary
        /// </summary>
        /// <param name="resource">the resource to be updated</param>
        /// <param name="browsedResource">the resource returned by browsing the plc - make sure the sub-Nodes are present (!)</param>
        void UpdateResource(ApiFileResource resource, ApiFileResource browsedResource);

    }
}