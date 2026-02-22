// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MITusing System.Threading.Tasks;
using Siemens.Simatic.S7.Webserver.API.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.WebApp
{
    /// <summary>
    /// Class used for the automated deployment of WebApps to a PLC (1500)
    /// </summary>
    public interface IApiWebAppDeployer
    {
        /// <summary>
        /// make very sure the given webapp contains all the data:
        /// Name
        /// ApplicationResources
        /// PathToWebAppDirectory
        /// </summary>
        /// <param name="webApp"><see cref="ApiWebAppData"/> - e.g. from parsed webappdirectory</param>
        void Deploy(ApiWebAppData webApp);
        /// <summary>
        /// make very sure the given webapp contains all the data:
        /// Name
        /// ApplicationResources
        /// PathToWebAppDirectory
        /// </summary>
        /// <param name="webApp"><see cref="ApiWebAppData"/> - e.g. from parsed webappdirectory</param>
        /// <param name="progress">Progress to report to</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        Task DeployAsync(ApiWebAppData webApp, IProgress<int> progress = null, CancellationToken cancellationToken = default);
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
        /// used to determine whether the deployer should retry a upload and compare of the resources found or give up right away
        /// </param>
        void DeployOrUpdate(ApiWebAppData webApp, int amountOfTriesForResourceDeployment = 1);
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
        /// used to determine whether the deployer should retry a upload and compare of the resources found or give up right away
        /// </param>
        /// /// <param name="progress">Progress to report to</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        Task DeployOrUpdateAsync(ApiWebAppData webApp, int amountOfTriesForResourceDeployment = 1, IProgress<int> progress = null, CancellationToken cancellationToken = default);
    }
}