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
        private const uint ChunkSize = 50;
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
        /// <param name="redundancyId">(optional) If the target is an S7-1500 R/H system, you can choose if you want to request the syslog of the primary or backup PLC</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The aggregated <see cref="ApiPlcSyslog"/> containing all currently available entries.</returns>
        public async Task<ApiPlcSyslog> RetrieveAllAsync(ApiPlcRedundancyId redundancyId = ApiPlcRedundancyId.StandardPLC, CancellationToken cancellationToken = default)
        {
            ApiPlcSyslog result = null;
            uint expectedTotal = 0;
            uint expectedLost = 0;
            uint availableCount = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                uint? first = null;
                uint requestedCount = ChunkSize;

                if (result != null)
                {
                    uint alreadyRetrieved = (uint)result.Entries.Count;
                    uint remaining = availableCount - alreadyRetrieved;
                    requestedCount = Math.Min(ChunkSize, remaining);
                    first = expectedTotal - alreadyRetrieved;
                }

                _logger?.LogTrace($"Requesting syslog browse: redundancyId={redundancyId}, count={requestedCount}, first={first}");
                var response = await _apiRequestHandler.ApiSyslogBrowseAsync(redundancyId, requestedCount, first, cancellationToken);
                var current = response?.Result ?? throw new InvalidOperationException("Syslog.Browse returned no result.");

                if (result == null)
                {
                    if (current.Count_Total < current.Count_Lost)
                    {
                        throw new InvalidOperationException("Syslog.Browse returned an invalid syslog count state (count_total < count_lost).");
                    }

                    expectedTotal = current.Count_Total;
                    expectedLost = current.Count_Lost;
                    availableCount = expectedTotal - expectedLost;

                    result = new ApiPlcSyslog
                    {
                        Count_Total = current.Count_Total,
                        Count_Lost = current.Count_Lost,
                        Entries = new List<ApiPlcSyslog_Entry>()
                    };

                    if (availableCount == 0)
                    {
                        return result;
                    }
                }
                else if (current.Count_Total != expectedTotal || current.Count_Lost != expectedLost)
                {
                    throw new InvalidOperationException("The syslog buffer changed while it was being retrieved.");
                }

                var currentEntries = current.Entries ?? new List<ApiPlcSyslog_Entry>();
                if (currentEntries.Count == 0)
                {
                    if (result.Entries.Count < availableCount)
                    {
                        throw new InvalidOperationException("Syslog.Browse made no progress while retrieving all entries.");
                    }
                    return result;
                }

                int remainingToTake = (int)(availableCount - (uint)result.Entries.Count);
                int takeCount = Math.Min(currentEntries.Count, remainingToTake);
                if (takeCount <= 0)
                {
                    return result;
                }

                result.Entries.AddRange(currentEntries.Take(takeCount));

                if (result.Entries.Count >= availableCount)
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
        public ApiPlcSyslog RetrieveAll(ApiPlcRedundancyId redundancyId = ApiPlcRedundancyId.StandardPLC)
            => RetrieveAllAsync(redundancyId).GetAwaiter().GetResult();
    }
}
