// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Moq;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Services.PlcProgram.Subscription;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Siemens.Simatic.S7.Webserver.API.Services.PlcProgram;
using Siemens.Simatic.S7.Webserver.API.Models.Responses;

namespace Webserver.API.UnitTests.SubscriptionFaker
{
    /// <summary>
    /// Test fixture providing mocks and helpers for ApiPlcProgramSubscriptionFaker testing.
    /// </summary>
    public class SubscriptionFakerTestFixture : IDisposable
    {
        public Mock<IApiRequestHandler> MockRequestHandler { get; }
        public Mock<IApiPlcProgramHandler> MockPlcProgramHandler { get; }
        public Mock<IApiRequestFactory> MockRequestFactory { get; }
        public Mock<ILogger> MockLogger { get; }
        
        public ApiPlcProgramSubscriptionFaker Subscription { get; }
        
        private List<ApiPlcProgramData> _moniteredItems = new List<ApiPlcProgramData>();
        private int _requestIdCounter = 0;

        public SubscriptionFakerTestFixture(int pollingIntervalMs = 100)
        {
            MockRequestHandler = new Mock<IApiRequestHandler>();
            MockPlcProgramHandler = new Mock<IApiPlcProgramHandler>();
            MockRequestFactory = new Mock<IApiRequestFactory>();
            MockLogger = new Mock<ILogger>();

            SetupDefaultMocks(pollingIntervalMs);

            Subscription = new ApiPlcProgramSubscriptionFaker(
                MockRequestHandler.Object,
                MockPlcProgramHandler.Object,
                MockRequestFactory.Object,
                MockLogger.Object,
                pollingIntervalMs);
        }

        private void SetupDefaultMocks(int pollingIntervalMs)
        {
            // Setup PlcProgramHandler to browse items without throwing
            MockPlcProgramHandler
                .Setup(m => m.PlcProgramBrowseSetChildrenAndParentsAsync(
                    It.IsAny<ApiPlcProgramBrowseMode>(),
                    It.IsAny<ApiPlcProgramData>(),
                    It.IsAny<System.Threading.CancellationToken>()))
                .Returns(Task.FromResult(new ApiPlcProgramBrowseResponse { Result = new List<ApiPlcProgramData>() }));

            // Setup RequestFactory - avoid optional parameters in Setup by using Callback pattern
            MockRequestFactory
                .Setup(m => m.GetApiPlcProgramReadRequest(It.IsAny<string>(), It.IsAny<ApiPlcDataRepresentation>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string varName, ApiPlcDataRepresentation mode, string jsonRpc, string id) => 
                    CreateMockRequest(varName));

            // Setup GetApiBulkRequestWithUniqueIds with all parameters
            MockRequestFactory
                .Setup(m => m.GetApiBulkRequestWithUniqueIds(It.IsAny<IEnumerable<IApiRequest>>(), It.IsAny<TimeSpan?>()))
                .Returns((IEnumerable<IApiRequest> requests, TimeSpan? timeout) =>
                    requests.Select((r, idx) =>
                    {
                        var mockReq = new Mock<IApiRequest>();
                        mockReq.Setup(x => x.Id).Returns((++_requestIdCounter).ToString());
                        mockReq.Setup(x => x.Params).Returns(r.Params);
                        return mockReq.Object;
                    }).ToList());

            // Setup default successful bulk response
            SetupSuccessfulBulkResponse();
        }

        /// <summary>
        /// Creates a mock ApiPlcProgramData with specified name and datatype.
        /// </summary>
        public ApiPlcProgramData CreateMockMonitoredItem(
            string name, 
            ApiPlcProgramDataType datatype = ApiPlcProgramDataType.Bool,
            int dbNumber = 1)
        {
            var item = new ApiPlcProgramData
            {
                Name = name,
                Datatype = datatype,
                Db_number = dbNumber,
                Value = null,
                Children = new List<ApiPlcProgramData>(),
                Parents = new List<ApiPlcProgramData>()
            };
            return item;
        }

        /// <summary>
        /// Sets up the mock to return successful bulk responses with provided values.
        /// </summary>
        public void SetupSuccessfulBulkResponse(Dictionary<string, object> valuesByRequestId = null)
        {
            MockRequestHandler
                .Setup(m => m.ApiBulkAsync(It.IsAny<IEnumerable<IApiRequest>>(), It.IsAny<System.Threading.CancellationToken>()))
                .Returns((IEnumerable<IApiRequest> requests, System.Threading.CancellationToken ct) =>
                {
                    var responses = requests.Select(r =>
                    {
                        var response = new ApiResultResponse<object>
                        {
                            Id = r.Id,
                            Result = valuesByRequestId?.ContainsKey(r.Id) == true 
                                ? valuesByRequestId[r.Id] 
                                : $"Value_{r.Id}"
                        };
                        return response;
                    }).ToList();

                    var bulkResponse = new ApiBulkResponse
                    {
                        SuccessfulResponses = responses,
                        ErrorResponses = new List<ApiErrorModel>()
                    };

                    return Task.FromResult(bulkResponse);
                });
        }

        /// <summary>
        /// Sets up the mock to simulate a timeout by throwing OperationCanceledException.
        /// </summary>
        public void SetupTimeoutBehavior()
        {
            MockRequestHandler
                .Setup(m => m.ApiBulkAsync(It.IsAny<IEnumerable<IApiRequest>>(), It.IsAny<System.Threading.CancellationToken>()))
                .Callback((IEnumerable<IApiRequest> _, System.Threading.CancellationToken ct) =>
                {
                    // Simulate timeout by checking if cancellation is requested
                    ct.ThrowIfCancellationRequested();
                })
                .Returns<IEnumerable<IApiRequest>, System.Threading.CancellationToken>((reqs, ct) =>
                {
                    ct.ThrowIfCancellationRequested();
                    throw new OperationCanceledException();
                });
        }

        /// <summary>
        /// Sets up the mock to simulate a slow poll that takes specified duration.
        /// </summary>
        public void SetupSlowPollBehavior(int delayMs)
        {
            MockRequestHandler
                .Setup(m => m.ApiBulkAsync(It.IsAny<IEnumerable<IApiRequest>>(), It.IsAny<System.Threading.CancellationToken>()))
                .Returns(async (IEnumerable<IApiRequest> requests, System.Threading.CancellationToken ct) =>
                {
                    await Task.Delay(delayMs, ct);
                    
                    var responses = requests.Select(r =>
                    {
                        var response = new ApiResultResponse<object>
                        {
                            Id = r.Id,
                            Result = $"Value_{r.Id}"
                        };
                        return response;
                    }).ToList();

                    var bulkResponse = new ApiBulkResponse
                    {
                        SuccessfulResponses = responses,
                        ErrorResponses = new List<ApiErrorModel>()
                    };

                    return bulkResponse;
                });
        }

        private IApiRequest CreateMockRequest(string varName)
        {
            var mockRequest = new Mock<IApiRequest>();
            mockRequest.Setup(x => x.Id).Returns((++_requestIdCounter).ToString());
            mockRequest.Setup(x => x.Params).Returns(new Dictionary<string, object> { { "var", varName } });
            return mockRequest.Object;
        }

        public void Dispose()
        {
            Subscription?.Dispose();
        }
    }
}
