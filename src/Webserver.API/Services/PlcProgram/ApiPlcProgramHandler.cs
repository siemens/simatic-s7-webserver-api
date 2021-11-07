// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Extensions;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Requests;
using Siemens.Simatic.S7.Webserver.API.Responses;
using Siemens.Simatic.S7.Webserver.API.Services.IdGenerator;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.PlcProgram
{
    /// <summary>
    /// Api PlcProgram Handler
    /// </summary>
    public class ApiPlcProgramHandler
    {
        private readonly IApiRequestHandler ApiRequestHandler;
        private readonly IIdGenerator IdGenerator;

        /// <summary>
        /// Api PlcProgram Handler
        /// </summary>
        /// <param name="asyncRequestHandler"></param>
        public ApiPlcProgramHandler(IApiRequestHandler asyncRequestHandler, IIdGenerator idGenerator)
        {
            this.ApiRequestHandler = asyncRequestHandler;
            this.IdGenerator = idGenerator;
        }

        /// <summary>
        /// If plcProgramBrowseMode == ApiPlcProgramBrowseMode.Children: ELSE "normal PlcProgramBrowse implementation"
        /// PlcProgramBrowse that will add the Elements from the response to the children of given var, also adds the parents of var to the list of parents of the children and also var as parent
        /// </summary>
        /// <param name="requestHandler">Api RequestHandler used to send the request</param>
        /// <param name="plcProgramBrowseMode">Mode for PlcProgramBrowse function</param>
        /// <param name="var">the db/structure of which the children should be browsed</param>
        /// <returns>ApiResultResponse of List of ApiPlcProgramData containing the children of the given var</returns>
        public async Task<ApiPlcProgramBrowseResponse> PlcProgramBrowseSetChildrenAndParentsAsync(ApiPlcProgramBrowseMode plcProgramBrowseMode, ApiPlcProgramData var)
        {
            var response = await ApiRequestHandler.PlcProgramBrowseAsync(plcProgramBrowseMode, var);
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
                    if (!var.Children.Any(child => child.Equals(el)))
                    {
                        var.Children.Add(el);
                    }
                });
            }
            return response;
        }

        /// <summary>
        /// Method to comfortably read all Children of a struct using a Bulk Request
        /// </summary>
        /// <param name="requestHandler">request Handler to use for the Api Bulk Request</param>
        /// <param name="structToRead">Struct of which the Children should be Read by Bulk Request</param>
        /// <param name="childrenReadMode">Mode in which the child values should be read - defaults to simple (easy user handling)</param>
        /// <param name="threadSleepTimeInMilliseconds">Time in milliseconds for the Thread to sleep in between creating Requests (=> so that new Ids will be generated)</param>
        /// <returns>The Struct containing the Children with their according Values</returns>
        public async Task<ApiPlcProgramData> PlcProgramReadStructByChildValuesAsync(ApiPlcProgramData structToRead, ApiPlcProgramReadOrWriteMode childrenReadMode = ApiPlcProgramReadOrWriteMode.Simple)
        {
            var toReturn = structToRead.ShallowCopy();
            if (toReturn.Children == null || toReturn.Children.Count == 0)
            {
                await PlcProgramBrowseSetChildrenAndParentsAsync(ApiPlcProgramBrowseMode.Children, toReturn);
            }
            List<ApiRequest> requests = new List<ApiRequest>();
            ApiRequestFactory factory = new ApiRequestFactory(IdGenerator);
            foreach (var child in toReturn.Children)
            {
                requests.Add(factory.GetApiPlcProgramReadRequest(child.GetVarNameForMethods(), childrenReadMode));
            }
            requests.MakeSureRequestIdsAreUnique(IdGenerator);
            var childvalues = await ApiRequestHandler.ApiBulkAsync(requests);
            foreach (var childval in childvalues.SuccessfulResponses)
            {
                var accordingRequest = requests.First(el => el.Id == childval.Id);
                var requestedVarString = accordingRequest.Params["var"];
                var childWithVarString = toReturn.Children.First(el => el.GetVarNameForMethods() == (string)requestedVarString);
                childWithVarString.Value = childval.Result;
            }
            return toReturn;
        }

        /// <summary>
        /// Method to comfortably write all Children of a struct using a Bulk Request
        /// </summary>
        /// <param name="requestHandler">request Handler to use for the Api Bulk Request</param>
        /// <param name="structToWrite">Struct of which the Children should be written by Bulk Request</param>
        /// <param name="childrenWriteMode">Mode in which the child values should be written - defaults to simple (easy user handling)</param>
        /// <param name="threadSleepTimeInMilliseconds">Time in milliseconds for the Thread to sleep in between creating Requests (=> so that new Ids will be generated)</param>
        /// <returns>The Struct containing the Children with their according Values</returns>
        public async Task<ApiBulkResponse> PlcProgramWriteStructByChildValuesAsync(ApiPlcProgramData structToWrite, ApiPlcProgramReadOrWriteMode childrenWriteMode = ApiPlcProgramReadOrWriteMode.Simple)
        {
            var toReturn = structToWrite.ShallowCopy();
            if (toReturn.Children == null || toReturn.Children.Count == 0)
            {
                throw new Exception($"No child elements present on var {toReturn.GetVarNameForMethods()}!");
            }
            List<ApiRequest> requests = new List<ApiRequest>();
            ApiRequestFactory factory = new ApiRequestFactory(IdGenerator);
            foreach (var child in toReturn.Children)
            {
                requests.Add(factory.GetApiPlcProgramWriteRequest(child.GetVarNameForMethods(), child.Value, childrenWriteMode));
            }
            requests.MakeSureRequestIdsAreUnique(IdGenerator);
            return await ApiRequestHandler.ApiBulkAsync(requests);
        }

    }
}
