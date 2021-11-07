// Copyright (c) 2021, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Requests;
using Siemens.Simatic.S7.Webserver.API.Services.IdGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Siemens.Simatic.S7.Webserver.API.Extensions
{
    /// <summary>
    /// Extensions on List of ApiRequest
    /// </summary>
    public static class ListOfApiRequestExtensions
    {
        /// <summary>
        /// Extension method to make sure the Ids of the List of ApiRequests contain no Id twice! 
        /// Not super performant but does what it should - might be buggy for some processors - possibility for own implementation is possible!
        /// </summary>
        /// <param name="requests">List of ApiRequests for which uniqueness should be made sure</param>
        /// <param name="requestIdGenerator">Request Id Generator - will default to ApiRequestIdGenerator (when null is given)</param>
        public static void MakeSureRequestIdsAreUnique(this List<ApiRequest> requests, IIdGenerator requestIdGenerator)
        {
            if(requestIdGenerator == null)
            {
                throw new ArgumentException(nameof(requestIdGenerator) + " was null!");
            }
            while (requests.GroupBy(el => el.Id).Count() != requests.Count)
            {
                requests.Where(el => requests.Any(el2 => el.Id == el2.Id))
                    .ToList().ForEach(el =>
                    {
                        el.Id = requestIdGenerator.Generate(8);
                    });
            }
        }
    }
}
