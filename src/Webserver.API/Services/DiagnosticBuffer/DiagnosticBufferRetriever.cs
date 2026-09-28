// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models.ApiDiagnosticBuffer;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.DiagnosticBuffer
{
    /// <summary>
    /// Retrieves all currently available diagnostic buffer entries.
    /// </summary>
    public class DiagnosticBufferRetriever
    {
        private const uint InitialCount = 50;
        private readonly IApiRequestHandler _apiRequestHandler;

        /// <summary>
        /// Creates a diagnostic buffer retriever using the provided request handler.
        /// </summary>
        /// <param name="apiRequestHandler">API request handler used for DiagnosticBuffer.Browse requests.</param>
        public DiagnosticBufferRetriever(IApiRequestHandler apiRequestHandler)
        {
            _apiRequestHandler = apiRequestHandler;
        }

        /// <summary>
        /// Retrieves a complete diagnostic buffer in at most two requests.
        /// Entries are returned from a single response, since the API exposes no snapshot cursor.
        /// A detected change between requests causes an InvalidOperationException.
        /// </summary>
        /// <param name="language">Language in which diagnostic texts shall be returned.</param>
        /// <param name="filters">Optional attributes filter for diagnostic buffer entries.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The complete diagnostic buffer from the last response.</returns>
        public async Task<ApiDiagnosticBuffer> RetrieveAllAsync(CultureInfo language,
                                                                 ApiDiagnosticBuffer_RequestFilters filters = null,
                                                                 CancellationToken cancellationToken = default)
        {
            var initial = await BrowseAsync(language, InitialCount, filters, cancellationToken);
            if (initial.Entries.Count == initial.Count_Current)
            {
                return initial;
            }

            var complete = await BrowseAsync(language, (uint)initial.Count_Current, filters, cancellationToken);
            if (complete.Count_Current != initial.Count_Current || complete.Last_Modified != initial.Last_Modified)
            {
                throw new InvalidOperationException("The diagnostic buffer changed while it was being retrieved.");
            }

            return complete;
        }

        private async Task<ApiDiagnosticBuffer> BrowseAsync(CultureInfo language, uint count,
            ApiDiagnosticBuffer_RequestFilters filters, CancellationToken cancellationToken)
        {
            var response = await _apiRequestHandler.ApiDiagnosticBufferBrowseAsync(language, count, filters, cancellationToken);
            var result = response?.Result ?? throw new InvalidOperationException("DiagnosticBuffer.Browse returned no result.");
            if (result.Count_Current < 0 || result.Count_Max < result.Count_Current)
            {
                throw new InvalidOperationException("DiagnosticBuffer.Browse returned invalid entry counts.");
            }

            result.Entries = result.Entries ?? new List<ApiDiagnosticBuffer_Entry>();
            if (result.Entries.Count != Math.Min((long)count, result.Count_Current))
            {
                throw new InvalidOperationException("DiagnosticBuffer.Browse returned an unexpected number of entries.");
            }

            return result;
        }

        /// <summary>
        /// Retrieves a complete diagnostic buffer in at most two requests.
        /// </summary>
        /// <param name="language">Language in which diagnostic texts shall be returned.</param>
        /// <param name="filters">Optional attributes filter for diagnostic buffer entries.</param>
        /// <returns>The complete diagnostic buffer from the last response.</returns>
        public ApiDiagnosticBuffer RetrieveAll(CultureInfo language, ApiDiagnosticBuffer_RequestFilters filters = null)
            => RetrieveAllAsync(language, filters).GetAwaiter().GetResult();
    }
}
