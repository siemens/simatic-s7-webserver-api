// Copyright (c) 2024, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Models.Responses;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.PlcProgram
{
    /// <summary>
    /// Api PlcProgram Handler
    /// </summary>
    public class ApiPlcProgramHandler : IApiPlcProgramHandler
    {
        private readonly IApiRequestHandler _apiRequestHandler;
        private readonly IApiRequestFactory _requestFactory;

        /// <summary>
        /// Api PlcProgram Handler
        /// </summary>
        /// <param name="asyncRequestHandler">Request Handler to send the Requests to the plc</param>
        /// <param name="requestFactory">Request Factory for request generation</param>
        public ApiPlcProgramHandler(IApiRequestHandler asyncRequestHandler, IApiRequestFactory requestFactory)
        {
            this._apiRequestHandler = asyncRequestHandler ?? throw new ArgumentNullException(nameof(asyncRequestHandler));
            this._requestFactory = requestFactory ?? throw new ArgumentNullException(nameof(requestFactory));
        }

        /// <summary>
        /// If plcProgramBrowseMode == ApiPlcProgramBrowseMode.Children: ELSE "normal PlcProgramBrowse implementation"
        /// PlcProgramBrowse that will add the Elements from the response to the children of given var, also adds the parents of var to the list of parents of the children and also var as parent
        /// </summary>
        /// <param name="plcProgramBrowseMode">Mode for PlcProgramBrowse function</param>
        /// <param name="var">the db/structure of which the children should be browsed</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>ApiResultResponse of List of ApiPlcProgramData containing the children of the given var</returns>
        public async Task<ApiPlcProgramBrowseResponse> PlcProgramBrowseSetChildrenAndParentsAsync(ApiPlcProgramBrowseMode plcProgramBrowseMode, ApiPlcProgramData var, CancellationToken cancellationToken = default(CancellationToken))
        {
            var response = await _apiRequestHandler.PlcProgramBrowseAsync(plcProgramBrowseMode, var, cancellationToken);
            if (plcProgramBrowseMode == ApiPlcProgramBrowseMode.Children)
            {
                response.Result.ForEach(el =>
                {
                    el.Parents = new List<ApiPlcProgramData>(var.Parents);
                    el.Parents.Add(var);
                    if (var.Children == null)
                    {
                        var.Children = new List<ApiPlcProgramData>();
                    }
                    if (el.ArrayElements?.Count != 0)
                    {
                        foreach (var arrayEl in el.ArrayElements)
                        {
                            arrayEl.Parents = el.Parents;
                        }
                    }
                    if (!var.Children.Any(child => child.Equals(el)))
                    {
                        var.Children.Add(el);
                    }
                });
            }
            return response;
        }

        /// <summary>
        /// If plcProgramBrowseMode == ApiPlcProgramBrowseMode.Children: ELSE "normal PlcProgramBrowse implementation"
        /// PlcProgramBrowse that will add the Elements from the response to the children of given var, also adds the parents of var to the list of parents of the children and also var as parent
        /// </summary>
        /// <param name="plcProgramBrowseMode">Mode for PlcProgramBrowse function</param>
        /// <param name="var">the db/structure of which the children should be browsed</param>
        /// <returns>ApiResultResponse of List of ApiPlcProgramData containing the children of the given var</returns>
        public ApiPlcProgramBrowseResponse PlcProgramBrowseSetChildrenAndParents(ApiPlcProgramBrowseMode plcProgramBrowseMode, ApiPlcProgramData var)
        => PlcProgramBrowseSetChildrenAndParentsAsync(plcProgramBrowseMode, var).GetAwaiter().GetResult();

        /// <summary>
        /// Method to comfortably read all Children of a struct using a Bulk Request
        /// </summary>
        /// <param name="structToRead">Struct of which the Children should be Read by Bulk Request</param>
        /// <param name="childrenReadMode">Mode in which the child values should be read - defaults to simple (easy user handling)</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>The Struct containing the Children with their according Values</returns>
        public async Task<ApiPlcProgramData> PlcProgramReadStructByChildValuesAsync(ApiPlcProgramData structToRead, ApiPlcDataRepresentation childrenReadMode = ApiPlcDataRepresentation.Simple
            , CancellationToken cancellationToken = default(CancellationToken))
        {
            var toReturn = structToRead.ShallowCopy();
            toReturn.Children = new List<ApiPlcProgramData>(structToRead.Children);
            toReturn.ArrayElements = new List<ApiPlcProgramData>(structToRead.ArrayElements);
            toReturn.Parents = new List<ApiPlcProgramData>(structToRead.Parents);
            if (toReturn.Children == null || toReturn.Children.Count == 0)
            {
                await PlcProgramBrowseSetChildrenAndParentsAsync(ApiPlcProgramBrowseMode.Children, toReturn, cancellationToken);
            }
            List<IApiRequest> requests = new List<IApiRequest>();
            foreach (var child in toReturn.Children)
            {
                if (!child.Datatype.IsSupportedByPlcProgramReadOrWrite())
                {
                    await PlcProgramReadStructByChildValuesAsync(child, childrenReadMode, cancellationToken);
                }
                else if (child.ArrayElements?.Count != 0)
                {
                    foreach (var arrayElement in child.ArrayElements)
                    {
                        if (!child.Datatype.IsSupportedByPlcProgramReadOrWrite())
                        {
                            await PlcProgramReadStructByChildValuesAsync(arrayElement, childrenReadMode, cancellationToken);
                        }
                        else
                        {
                            requests.Add(_requestFactory.GetApiPlcProgramReadRequest(arrayElement.GetVarNameForMethods(), childrenReadMode));
                        }
                    }
                }
                else if (child.Children?.Count == 0)
                {
                    requests.Add(_requestFactory.GetApiPlcProgramReadRequest(child.GetVarNameForMethods(), childrenReadMode));
                }
                else
                {
                    throw new Exception("The current PlcProgramData Element does not have children, " +
                        "neither it has ArrayElements but still it is not supported by plcprogram " +
                        "read or write! should not be the case... please open an issue on https://github.com/siemens/simatic-s7-webserver-api/issues.");
                }
            }
            requests = _requestFactory.GetApiBulkRequestWithUniqueIds(requests).ToList();
            if (requests.Count > 0)
            {
                var childvalues = await _apiRequestHandler.ApiBulkAsync(requests, cancellationToken);
                foreach (var childval in childvalues.SuccessfulResponses)
                {
                    var accordingRequest = requests.First(el => el.Id == childval.Id);
                    var requestedVarString = accordingRequest.Params["var"];
                    var childOrArrayElementWithVarString = toReturn.Children.FirstOrDefault(el => el.GetVarNameForMethods() == (string)requestedVarString) ??
                        (toReturn.Children.First(el => el.ArrayElements.Any(arrEl => arrEl.GetVarNameForMethods() == (string)requestedVarString))
                            .ArrayElements.First(arrEl => arrEl.GetVarNameForMethods() == (string)requestedVarString));
                    childOrArrayElementWithVarString.Value = childval.Result;
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Method to comfortably read all Children of a struct using a Bulk Request
        /// </summary>
        /// <param name="structToRead">Struct of which the Children should be Read by Bulk Request</param>
        /// <param name="childrenReadMode">Mode in which the child values should be read - defaults to simple (easy user handling)</param>
        /// <returns>The Struct containing the Children with their according Values</returns>
        public ApiPlcProgramData PlcProgramReadStructByChildValues(ApiPlcProgramData structToRead, ApiPlcDataRepresentation childrenReadMode = ApiPlcDataRepresentation.Simple)
            => PlcProgramReadStructByChildValuesAsync(structToRead, childrenReadMode).GetAwaiter().GetResult();

        /// <summary>
        /// Method to comfortably write all Children of a struct using a Bulk Request
        /// </summary>
        /// <param name="structToWrite">Struct of which the Children should be written by Bulk Request</param>
        /// <param name="childrenWriteMode">Mode in which the child values should be written - defaults to simple (easy user handling)</param>
        /// <param name="cancellationToken">Enables the method to terminate its operation if a cancellation is requested from it's CancellationTokenSource.</param>
        /// <returns>The Struct containing the Children with their according Values</returns>
        public async Task<ApiBulkResponse> PlcProgramWriteStructByChildValuesAsync(ApiPlcProgramData structToWrite, ApiPlcDataRepresentation childrenWriteMode = ApiPlcDataRepresentation.Simple, CancellationToken cancellationToken = default(CancellationToken))
        {
            var toReturn = structToWrite.ShallowCopy();
            if (toReturn.Children == null || toReturn.Children.Count == 0)
            {
                throw new Exception($"No child elements present on var {toReturn.GetVarNameForMethods()}!");
            }
            List<IApiRequest> requests = new List<IApiRequest>();
            foreach (var child in toReturn.Children)
            {
                requests.Add(_requestFactory.GetApiPlcProgramWriteRequest(child.GetVarNameForMethods(), child.Value, childrenWriteMode));
            }
            requests = _requestFactory.GetApiBulkRequestWithUniqueIds(requests).ToList();
            return await _apiRequestHandler.ApiBulkAsync(requests, cancellationToken);
        }

        /// <summary>
        /// Method to comfortably write all Children of a struct using a Bulk Request
        /// </summary>
        /// <param name="structToWrite">Struct of which the Children should be written by Bulk Request</param>
        /// <param name="childrenWriteMode">Mode in which the child values should be written - defaults to simple (easy user handling)</param>
        /// <returns>The Struct containing the Children with their according Values</returns>
        public ApiBulkResponse PlcProgramWriteStructByChildValues(ApiPlcProgramData structToWrite, ApiPlcDataRepresentation childrenWriteMode = ApiPlcDataRepresentation.Simple)
            => PlcProgramWriteStructByChildValuesAsync(structToWrite, childrenWriteMode).GetAwaiter().GetResult();
    }
}
