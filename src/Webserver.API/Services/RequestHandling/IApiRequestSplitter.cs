// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using System.Collections.Generic;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary>
    /// Split the provided Bulk Request into multiple Message chunks
    /// </summary>
    public interface IApiRequestSplitter
    {
        /// <summary>
        /// Get Requests (byte[] to send to the server) for the given api Requests so that the byte[] built will be within the MaxRequestSize
        /// </summary>
        /// <param name="apiRequests">Api Requests to be sent</param>
        /// <param name="MaxRequestSize">Max Request size for the Bulk Requests to be created.</param>
        /// <returns>Bulk requests ByteArrayContent to be sent</returns>
        IEnumerable<byte[]> GetMessageChunks(IEnumerable<IApiRequest> apiRequests, long MaxRequestSize);
    }
}
