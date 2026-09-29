// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models.ApiSyslog;
using System.Threading;
using System.Threading.Tasks;

namespace Siemens.Simatic.S7.Webserver.API.Services.Syslog
{
    /// <summary>
    /// Handler to gather all currently available syslog events from the PLC.
    /// </summary>
    public interface ISyslogHandler
    {
        /// <summary>
        /// Retrieves all currently available syslog events from the PLC-internal ring buffer.
        /// </summary>
        /// <remarks>
        /// Returns entries newest first. Changes to either cumulative counter during retrieval
        /// cause an InvalidOperationException; no partial result is returned. The caller controls retries.
        /// Completion requires reaching ID 1 or receiving as many entries as the cumulative counters
        /// allow at most. A response with no new entries before that proof causes an InvalidOperationException,
        /// including an anchor-only response at an otherwise unknown oldest retained boundary.
        /// </remarks>
        /// <param name="redundancyId">(optional) If the target is an S7-1500 R/H system, you can choose if you want to request the syslog of the primary or backup PLC</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>The aggregated <see cref="ApiPlcSyslog"/> containing all currently available entries.</returns>
        Task<ApiPlcSyslog> RetrieveAllAsync(ApiPlcRedundancyId redundancyId = ApiPlcRedundancyId.StandardPLC, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all currently available syslog events from the PLC-internal ring buffer.
        /// </summary>
        /// <remarks>Uses the same ordering and buffer-change policy as RetrieveAllAsync.</remarks>
        /// <param name="redundancyId">(optional) If the target is an S7-1500 R/H system, you can choose if you want to request the syslog of the primary or backup PLC</param>
        /// <returns>The aggregated <see cref="ApiPlcSyslog"/> containing all currently available entries.</returns>
        ApiPlcSyslog RetrieveAll(ApiPlcRedundancyId redundancyId = ApiPlcRedundancyId.StandardPLC);
    }
}
