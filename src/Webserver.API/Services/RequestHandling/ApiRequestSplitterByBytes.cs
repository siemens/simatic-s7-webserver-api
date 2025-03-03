using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Siemens.Simatic.S7.Webserver.API.StaticHelpers;
using Siemens.Simatic.S7.Webserver.API.Exceptions;

namespace Siemens.Simatic.S7.Webserver.API.Services.RequestHandling
{
    public class ApiRequestSplitterByBytes : IApiRequestSplitter
    {
        private readonly ILogger _logger;
        public ApiRequestSplitterByBytes(ILogger logger = null)
        {
            _logger = logger;
        }

        public IEnumerable<byte[]> GetMessageChunks(IEnumerable<IApiRequest> apiRequests, long MaxRequestSize)
        {
            foreach (var apiRequest in apiRequests)
            {
                if (apiRequest.Params != null)
                {
                    apiRequest.Params = apiRequest.Params
                        .Where(el => el.Value != null)
                        .ToDictionary(x => x.Key, x => x.Value);
                }
            }
            string apiRequestString = JsonConvert.SerializeObject(apiRequests, new JsonSerializerSettings()
            { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
            byte[] byteArr = Encoding.UTF8.GetBytes(apiRequestString);
            var messageChunks = new List<byte[]>();
            if (byteArr.Length >= MaxRequestSize)
            {
                // comma needs to be added for multiple requests within Bulk
                var commaBytes = Encoding.UTF8.GetBytes(",");
                var beginBytes = Encoding.UTF8.GetBytes("[");
                var endBytes = Encoding.UTF8.GetBytes("]");
                _logger?.LogInformation($"Chunk the Requests into multiple Bulk Requests '{byteArr.Length}' is > '{MaxRequestSize}' -> split into sub requests.");
                var chunkLenSum = 0;
                var commaMissingSum = 0;
                var currentStream = new MemoryStream();
                foreach (var request in apiRequests)
                {
                    string requestString = JsonConvert.SerializeObject(request, new JsonSerializerSettings()
                    { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
                    byte[] requestByteArr = Encoding.UTF8.GetBytes(requestString);
                    if (requestByteArr.Length > MaxRequestSize)
                    {
                        throw new ApiRequestBiggerThanMaxMessageSizeException($"Request Size '{requestByteArr.Length}' is bigger than the " +
                            $"MaxRequestSize '{MaxRequestSize}'! -> Not Possible to Chunk this Message!");
                    }
                    if (currentStream.Length + requestByteArr.Length + commaBytes.Length < MaxRequestSize)
                    {
                        currentStream.Append(commaBytes);
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
                // check for differences between buffers and the 'original' byte array
                if ((chunkLenSum + commaMissingSum) != byteArr.Length)
                {
                    throw new InvalidOperationException($"Programming error in Message Chunking -> chunks len together: '{chunkLenSum}' but they should be '{byteArr.Length}'!");
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
