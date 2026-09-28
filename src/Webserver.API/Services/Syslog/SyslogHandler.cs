// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.Logging;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models.ApiSyslog;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.Syslog
{
    /// <summary>
    /// Handler to gather all currently available syslog events from the PLC by repeatedly browsing the ring buffer.
    /// </summary>
    public class SyslogHandler : ISyslogHandler
    {
        // The Syslog.Browse request table specifies a maximum of 20 entries.
        private const uint ChunkSize = 20;
        private readonly IApiRequestHandler _apiRequestHandler;
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new <see cref="SyslogHandler"/>.
        /// </summary>
        /// <param name="apiRequestHandler">API request handler used for Syslog.Browse requests.</param>
        /// <param name="logger">Optional logger.</param>
        public SyslogHandler(IApiRequestHandler apiRequestHandler, ILogger logger = null)
        {
            _apiRequestHandler = apiRequestHandler ?? throw new ArgumentNullException(nameof(apiRequestHandler));
            _logger = logger;
        }

        /// <summary>
        /// Retrieves all currently available syslog events from the PLC-internal ring buffer.
        /// </summary>
        /// <remarks>
        /// Entries are returned newest first. Each continuation includes the previous page's
        /// last entry; a page containing only that entry marks the end of the retained buffer.
        /// Short pages with additional entries are followed until this boundary is reached.
        /// Count_Total and Count_Lost are cumulative counters, not the current buffer size.
        /// If either counter changes during retrieval, the operation fails without returning
        /// a partial result. The caller may retry with its own cancellation or timeout policy.
        /// </remarks>
        /// <param name="redundancyId">(optional) If the target is an S7-1500 R/H system, you can choose if you want to request the syslog of the primary or backup PLC</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The aggregated <see cref="ApiPlcSyslog"/> containing all currently available entries.</returns>
        /// <exception cref="InvalidOperationException">The buffer changed or a response is inconsistent.</exception>
        /// <exception cref="OperationCanceledException">The operation was canceled.</exception>
        public async Task<ApiPlcSyslog> RetrieveAllAsync(ApiPlcRedundancyId redundancyId = ApiPlcRedundancyId.StandardPLC, CancellationToken cancellationToken = default)
        {
            ApiPlcSyslog result = null;
            uint? first = null;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                uint requestedCount = Math.Min(ChunkSize, first ?? ChunkSize);

                _logger?.LogTrace($"Requesting syslog browse: redundancyId={redundancyId}, count={requestedCount}, first={first}");
                var response = await _apiRequestHandler.ApiSyslogBrowseAsync(redundancyId, requestedCount, first, cancellationToken).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
                var current = response?.Result ?? throw new InvalidOperationException("Syslog.Browse returned no result.");
                var entries = current.Entries;
                uint pageFirst = first ?? current.Count_Total;
                if (current.Count_Total < current.Count_Lost || entries == null ||
                    entries.Count > requestedCount || (uint)entries.Count > pageFirst ||
                    entries.Any(entry => entry == null || entry.Raw == null))
                {
                    throw new InvalidOperationException("Syslog.Browse returned an inconsistent response.");
                }

                if (result == null)
                {
                    result = new ApiPlcSyslog
                    {
                        Count_Total = current.Count_Total,
                        Count_Lost = current.Count_Lost,
                        Entries = new List<ApiPlcSyslog_Entry>()
                    };
                }
                else if (current.Count_Total != result.Count_Total || current.Count_Lost != result.Count_Lost)
                {
                    throw new InvalidOperationException("The syslog buffer changed while it was being retrieved.");
                }

                int skip = first.HasValue ? 1 : 0;
                if (first.HasValue && (entries.Count == 0 ||
                    entries[0].Raw != result.Entries[result.Entries.Count - 1].Raw))
                {
                    throw new InvalidOperationException("Syslog.Browse did not return the continuation entry.");
                }

                // Lost counts only overwritten entries that were NOT saved to a syslog
                // server. Total - lost is an upper bound, never a completion target.
                if ((ulong)result.Entries.Count + (uint)(entries.Count - skip) >
                    (ulong)current.Count_Total - current.Count_Lost)
                {
                    throw new InvalidOperationException("Syslog.Browse returned more entries than its counters allow.");
                }

                if (entries.Count == skip)
                {
                    return result;
                }

                result.Entries.AddRange(entries.Skip(skip));
                // Re-read the last known entry, not an ID older than the retained buffer.
                // Only page-sized values are converted; cumulative counters remain uint.
                first = pageFirst - (uint)(entries.Count - 1);
                if (first == 1)
                {
                    return result;
                }
            }
        }

        /// <summary>
        /// Retrieves all currently available syslog events from the PLC-internal ring buffer.
        /// </summary>
        /// <param name="redundancyId">(optional) If the target is an S7-1500 R/H system, you can choose if you want to request the syslog of the primary or backup PLC</param>
        /// <returns>The aggregated <see cref="ApiPlcSyslog"/> containing all currently available entries.</returns>
        /// <remarks>Runs the asynchronous call chain on the thread pool to avoid blocking a captured caller context.</remarks>
        public ApiPlcSyslog RetrieveAll(ApiPlcRedundancyId redundancyId = ApiPlcRedundancyId.StandardPLC)
            => Task.Run(() => RetrieveAllAsync(redundancyId)).GetAwaiter().GetResult();
    }
}
