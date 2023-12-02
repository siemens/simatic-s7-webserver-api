// Copyright (c) 2023, Siemens AG
//
// SPDX-License-Identifier: MITusing System.Threading.Tasks;
using Siemens.Simatic.S7.Webserver.API.Models;
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
        Task DeployAsync(ApiWebAppData webApp, CancellationToken cancellationToken = default(CancellationToken));
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
        /// used to determine wether the deployer should retry a upload and compare of the resources found or give up right away
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
        /// used to determine wether the deployer should retry a upload and compare of the resources found or give up right away
        /// </param>
        Task DeployOrUpdateAsync(ApiWebAppData webApp, int amountOfTriesForResourceDeployment = 1, CancellationToken cancellationToken = default(CancellationToken));
    }
}