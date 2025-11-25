// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Siemens.Simatic.S7.Webserver.API.Exceptions;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary cref="IApiRequestSplitter" />
    public class ApiRequestSplitter : IApiRequestSplitter
    {
        private readonly ILogger _logger;


        /// <summary cref="IApiRequestSplitter" />
        /// <param name="logger">Logger for the Request Splitter</param>
        public ApiRequestSplitter(ILogger logger = null)
        {
            _logger = logger;
        }

        /// <summary cref="GetMessageChunks(IEnumerable{IApiRequest}, long)" />
        public IEnumerable<byte[]> GetMessageChunks(IEnumerable<IApiRequest> apiRequests, long MaxRequestSize)
        {
            // Clean up null values from Params.
            foreach (var apiRequest in apiRequests)
            {
                if (apiRequest.Params != null)
                {
                    apiRequest.Params = apiRequest.Params
                        .Where(el => el.Value != null)
                        .ToDictionary(x => x.Key, x => x.Value);
                }
            }

            // Setup JSON serialization settings.
            var jsonSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            // Serialize each individual request.
            var serializedRequests = apiRequests
                .Select(request => JsonConvert.SerializeObject(request, jsonSettings))
                .ToList();

            // Build the full JSON array and determine its byte length.
            string fullRequestJson = "[" + string.Join(",", serializedRequests) + "]";
            byte[] fullRequestBytes = Encoding.UTF8.GetBytes(fullRequestJson);

            var messageChunks = new List<byte[]>();


            // If the full request is within the size limit, send it as one chunk.
            if (fullRequestBytes.Length < MaxRequestSize)
            {
                messageChunks.Add(fullRequestBytes);
            }
            else
            {
                _logger?.LogInformation($"Chunking requests: total size {fullRequestBytes.Length} bytes exceeds maximum allowed {MaxRequestSize} bytes.");

                // Build chunks that are valid JSON arrays.
                var currentChunkRequests = new List<string>();
                foreach (var requestJson in serializedRequests)
                {
                    // Create a candidate chunk that would include the next request.
                    string candidateChunk = "[" + string.Join(",", currentChunkRequests.Concat(new[] { requestJson })) + "]";
                    byte[] candidateBytes = Encoding.UTF8.GetBytes(candidateChunk);

                    // If adding the next request exceeds the max, finish this chunk.
                    if (candidateBytes.Length > MaxRequestSize)
                    {
                        if (!currentChunkRequests.Any())
                        {
                            // If a single request exceeds the limit, throw.
                            byte[] singleRequestBytes = Encoding.UTF8.GetBytes("[" + requestJson + "]");
                            throw new ApiRequestBiggerThanMaxMessageSizeException($"Request size {singleRequestBytes.Length} exceeds MaxRequestSize {MaxRequestSize}.");
                        }
                        // Finalize the current chunk.
                        string chunkJson = "[" + string.Join(",", currentChunkRequests) + "]";
                        var chunkBytes = Encoding.UTF8.GetBytes(chunkJson);

                        if (chunkBytes.Length > MaxRequestSize)
                        {
                            // If a single request exceeds the limit, throw.
                            throw new ApiRequestBiggerThanMaxMessageSizeException($"Request size {chunkBytes.Length} exceeds MaxRequestSize {MaxRequestSize}.");
                        }
                        messageChunks.Add(chunkBytes);

                        // Start a new chunk with the current request.
                        currentChunkRequests = new List<string> { requestJson };
                    }
                    else
                    {
                        // Otherwise, add the request to the current chunk.
                        currentChunkRequests.Add(requestJson);
                    }
                }
                // Add any remaining requests as the last chunk.
                if (currentChunkRequests.Any())
                {
                    string chunkJson = "[" + string.Join(",", currentChunkRequests) + "]";
                    var lastChunkBytes = Encoding.UTF8.GetBytes(chunkJson);
                    messageChunks.Add(lastChunkBytes);
                }
            }
            return messageChunks;
        }
    }
}
