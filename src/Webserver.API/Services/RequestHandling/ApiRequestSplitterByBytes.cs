﻿// Copyright (c) 2025, Siemens AG
//
// SPDX-License-Identifier: MIT
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using Siemens.Simatic.S7.Webserver.API.Exceptions;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    /// <summary cref="IApiRequestSplitter" />
    public class ApiRequestSplitterByBytes : IApiRequestSplitter
    {
        private readonly ILogger _logger;

        /// <summary cref="IApiRequestSplitter" />
        public ApiRequestSplitterByBytes(ILogger logger = null)
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
            var jsonSettings = new JsonSerializerSettings()
            { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() };
            // serialize all
            string apiRequestString = JsonConvert.SerializeObject(apiRequests, jsonSettings);
            byte[] byteArr = Encoding.UTF8.GetBytes(apiRequestString);
            var messageChunks = new List<byte[]>();
            // in case the message would be bigger than the max size -> chunk them
            if (byteArr.Length >= MaxRequestSize)
            {
                // comma needs to be added for multiple requests within Bulk, [ at start, ] at end
                var commaBytes = Encoding.UTF8.GetBytes(",");
                var beginBytes = Encoding.UTF8.GetBytes("[");
                var endBytes = Encoding.UTF8.GetBytes("]");
                _logger?.LogInformation($"Chunk the Requests into multiple Bulk Requests '{byteArr.Length}' is > '{MaxRequestSize}' -> split into sub requests.");
                var chunkLenSum = 0;
                var commaMissingSum = 0;
                var currentStream = new MemoryStream();
                currentStream.Append(beginBytes);
                foreach (var request in apiRequests)
                {
                    string requestString = JsonConvert.SerializeObject(request, jsonSettings);
                    byte[] requestByteArr = Encoding.UTF8.GetBytes(requestString);
                    if (requestByteArr.Length > MaxRequestSize)
                    {
                        throw new ApiRequestBiggerThanMaxMessageSizeException($"Request size {requestByteArr.Length} exceeds MaxRequestSize {MaxRequestSize}.");
                    }
                    if (currentStream.Length + requestByteArr.Length + commaBytes.Length < MaxRequestSize)
                    {
                        if(currentStream.Length > 5)
                        {
                            // only append the comma if there already is one inside
                            currentStream.Append(commaBytes);
                        }
                        currentStream.Append(requestByteArr); // append it to the current stream
                    }
                    else // save the current stream, add the current request to a new byte array
                    {
                        currentStream.Append(endBytes);
                        var currentBuffer = currentStream.ToArray();
                        messageChunks.Add(currentBuffer);
                        chunkLenSum += currentBuffer.Length;
                        currentStream = new MemoryStream();
                        currentStream.Append(beginBytes);
                        currentStream.Append(requestByteArr);
                        commaMissingSum += commaBytes.Length;
                    }
                }
                if(currentStream.Length > 0)
                {
                    // save the current (last) stream
                    currentStream.Append(endBytes);
                    var currentBufferAfterwards = currentStream.ToArray();
                    messageChunks.Add(currentBufferAfterwards);
                    chunkLenSum += currentBufferAfterwards.Length;
                    commaMissingSum += commaBytes.Length;
                }
            }
            else
            {
                messageChunks.Add(byteArr);
            }
            return messageChunks;
        }
    }
}
