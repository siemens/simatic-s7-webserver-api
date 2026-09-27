// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Models.ApiDiagnosticBuffer;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.DiagnosticBuffer
{
    /// <summary>
    /// Retrieves all currently available diagnostic buffer entries in bounded chunks.
    /// </summary>
    public class DiagnosticBufferRetriever
    {
        private const uint ChunkSize = 50;
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
        /// Retrieves all diagnostic buffer entries available at the start of the operation.
        /// The API exposes no offset/cursor, so the requested count is increased by 50 and
        /// only the newly exposed tail is appended on every request.
        /// </summary>
        /// <param name="language">Language in which diagnostic texts shall be returned.</param>
        /// <param name="filters">Optional attributes filter for diagnostic buffer entries.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The aggregated diagnostic buffer.</returns>
        public async Task<ApiDiagnosticBuffer> RetrieveAllAsync(CultureInfo language,
                                                                 ApiDiagnosticBuffer_RequestFilters filters = null,
                                                                 CancellationToken cancellationToken = default)
        {
            ApiDiagnosticBuffer result = null;
            int expectedCount = -1;
            DateTime expectedLastModified = default;
            uint requestedCount = ChunkSize;

            while (true)
            {
                var response = await _apiRequestHandler.ApiDiagnosticBufferBrowseAsync(language, requestedCount, filters, cancellationToken);
                var current = response?.Result ?? throw new InvalidOperationException("DiagnosticBuffer.Browse returned no result.");
                var currentEntries = current.Entries ?? new List<ApiDiagnosticBuffer_Entry>();

                if (result == null)
                {
                    if (current.Count_Current < 0)
                    {
                        throw new InvalidOperationException("DiagnosticBuffer.Browse returned a negative count_current value.");
                    }

                    expectedCount = current.Count_Current;
                    expectedLastModified = current.Last_Modified;
                    result = new ApiDiagnosticBuffer
                    {
                        Language = current.Language,
                        Last_Modified = current.Last_Modified,
                        Count_Current = current.Count_Current,
                        Count_Max = current.Count_Max,
                        Entries = new List<ApiDiagnosticBuffer_Entry>()
                    };
                }
                else if (current.Count_Current != expectedCount || current.Last_Modified != expectedLastModified)
                {
                    throw new InvalidOperationException("The diagnostic buffer changed while it was being retrieved.");
                }

                if (expectedCount == 0)
                {
                    return result;
                }

                var alreadyRetrieved = result.Entries.Count;
                var availableForSnapshot = Math.Min(currentEntries.Count, expectedCount);
                if (availableForSnapshot <= alreadyRetrieved)
                {
                    throw new InvalidOperationException("DiagnosticBuffer.Browse made no progress while retrieving all entries.");
                }

                result.Entries.AddRange(currentEntries
                    .Skip(alreadyRetrieved)
                    .Take(availableForSnapshot - alreadyRetrieved));

                if (result.Entries.Count >= expectedCount)
                {
                    return result;
                }

                requestedCount = (uint)Math.Min((long)expectedCount, (long)requestedCount + ChunkSize);
            }
        }

        /// <summary>
        /// Retrieves all diagnostic buffer entries available at the start of the operation.
        /// </summary>
        /// <param name="language">Language in which diagnostic texts shall be returned.</param>
        /// <param name="filters">Optional attributes filter for diagnostic buffer entries.</param>
        /// <returns>The aggregated diagnostic buffer.</returns>
        public ApiDiagnosticBuffer RetrieveAll(CultureInfo language, ApiDiagnosticBuffer_RequestFilters filters = null)
            => RetrieveAllAsync(language, filters).GetAwaiter().GetResult();
    }
}
