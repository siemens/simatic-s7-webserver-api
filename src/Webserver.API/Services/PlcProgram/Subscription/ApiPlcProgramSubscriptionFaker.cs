// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using Microsoft.Extensions.Logging;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Models.Requests;
using Siemens.Simatic.S7.Webserver.API.Services.RequestHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Siemens.Simatic.S7.Webserver.API.Services.PlcProgram.Subscription
{
    /// <summary>
    /// Class to fake a subscription (as long as the web-api does not natively support them).
    /// </summary>
    public class ApiPlcProgramSubscriptionFaker : IDisposable
    {
        private readonly List<ApiPlcProgramData> _monitoredItems;
        private System.Threading.Timer _pollingTimer;
        private int _isPolling = 0;
        private bool _isDisposed = false;
        private int _consecutiveSlowPolls = 0;

        /// <summary>
        /// Event raised when a monitored item's value changes.
        /// </summary>
        public event EventHandler<MonitoredItemValueChangedEventArgs> MonitoredItemValueChanged;

        /// <summary>
        /// Event raised when a polling cycle completes, containing all changes that occurred during the cycle.
        /// </summary>
        public event EventHandler<PollingCycleCompletedEventArgs> PollingCycleCompleted;

        /// <summary>
        /// Monitored items of the subscription. 
        /// These are updated and contain the current values of the monitored items.
        /// </summary>
        public IReadOnlyCollection<ApiPlcProgramData> MonitoredItems => _monitoredItems;

        /// <summary>
        /// Request handler - authorized Session
        /// </summary>
        public readonly IApiRequestHandler _apiRequestHandler;

        /// <summary>
        /// PlcProgram handler - structs
        /// </summary>
        public readonly IApiPlcProgramHandler _plcProgramHandler;

        /// <summary>
        /// Api Request Factory - to build the bulk requests
        /// </summary>
        public readonly IApiRequestFactory _requestFactory;

        /// <summary>
        /// Logger for logging subscription related information and errors.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Interval in milliseconds at which the Monitored Items will be requested from the PLC and updated in the MonitoredItems collection. Default is 1000ms (1 second).
        /// </summary>
        public int PollingInterval { get; set; } = 1000;

        /// <summary>
        /// Maximum exponential backoff multiplier when polls consistently exceed the polling interval.
        /// Default is 10x, meaning the maximum delay will be PollingInterval * 10.
        /// </summary>
        public int MaxBackoffMultiplier { get; set; } = 10;

        /// <summary>
        /// Indicates whether the subscription is currently active and polling.
        /// </summary>
        public bool IsRunning => _pollingTimer != null;

        /// <summary>
        /// Class to fake a subscription (as long as the web-api does not natively support them).
        /// </summary>
        /// <param name="requestHandler">Request handler for API calls</param>
        /// <param name="plcProgramHandler">PlcProgram handler for struct handling</param>
        /// <param name="requestFactory">Request factory to build API requests</param>
        /// <param name="logger">Logger for logging (optional)</param>
        /// <param name="pollingInterval">Polling interval in milliseconds (default: 1000ms)</param>
        public ApiPlcProgramSubscriptionFaker(IApiRequestHandler requestHandler, IApiPlcProgramHandler plcProgramHandler, IApiRequestFactory requestFactory, ILogger logger = null, int pollingInterval = 1000)
        {
            _apiRequestHandler = requestHandler ?? throw new ArgumentNullException(nameof(requestHandler));
            _plcProgramHandler = plcProgramHandler ?? throw new ArgumentNullException(nameof(plcProgramHandler));
            _requestFactory = requestFactory ?? throw new ArgumentNullException(nameof(requestFactory));
            _logger = logger;
            _monitoredItems = new List<ApiPlcProgramData>();
            PollingInterval = pollingInterval;
        }

        /// <summary>
        /// Add a monitored item to the subscription. The item will be updated in the configured PollingInterval.
        /// </summary>
        /// <param name="item">item to be added to monitoring</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task AddMonitoredItemAsync(ApiPlcProgramData item, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            // initialize the item when necessary
            if(item.Children == null || item.Children.Count == 0 && !item.Datatype.IsSupportedByPlcProgramReadOrWrite())
            {
                _logger?.LogDebug($"{nameof(ApiPlcProgramSubscriptionFaker)}: Need to initialize monitored item: {item.GetVarNameForMethods()}!");
                item.Children = new List<ApiPlcProgramData>(item.Children);
                item.ArrayElements = new List<ApiPlcProgramData>(item.ArrayElements);
                item.Parents = new List<ApiPlcProgramData>(item.Parents);
                if (item.Children == null || item.Children.Count == 0)
                {
                    await _plcProgramHandler.PlcProgramBrowseSetChildrenAndParentsAsync(ApiPlcProgramBrowseMode.Children, item, cancellationToken);
                }
                _logger?.LogDebug($"{nameof(ApiPlcProgramSubscriptionFaker)}: Initialized monitored item: {item.GetVarNameForMethods()}!");
            }
            cancellationToken.ThrowIfCancellationRequested();
            lock (_monitoredItems)
            {
                _monitoredItems.Add(item);
            }
        }

        /// <summary>
        /// Remove a monitored item from the subscription.
        /// </summary>
        /// <param name="item">item to be removed from monitoring</param>
        /// <returns>true if the item was removed, false if it was not found</returns>
        public bool DeleteMonitoredItem(ApiPlcProgramData item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            lock (_monitoredItems)
            {
                return _monitoredItems.Remove(item);
            }
        }

        /// <summary>
        /// Start the subscription polling. The monitored items will be read from the PLC at the configured PollingInterval.
        /// </summary>
        public void Start()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(ApiPlcProgramSubscriptionFaker));

            if (_pollingTimer != null)
                return;

            _pollingTimer = new System.Threading.Timer(PollingTimerCallback, null, 0, Timeout.Infinite);
        }

        /// <summary>
        /// Stop the subscription polling.
        /// </summary>
        public void Stop()
        {
            if (_pollingTimer != null)
            {
                _pollingTimer.Dispose();
                _pollingTimer = null;
            }
            _consecutiveSlowPolls = 0;
        }

        private void PollingTimerCallback(object state)
        {
            if (Interlocked.CompareExchange(ref _isPolling, 1, 0) == 0)
            {
                var pollMonitoredItemsStarted = DateTime.UtcNow;
                var changedItemsList = new List<(ApiPlcProgramData item, object oldValue, object newValue)>();
                try
                {
                    changedItemsList = PollMonitoredItemsAsync().GetAwaiter().GetResult();
                }
                finally
                {
                    var pollDuration = (DateTime.UtcNow - pollMonitoredItemsStarted).TotalMilliseconds;
                    if (changedItemsList.Count > 0 || PollingCycleCompleted != null)
                    {
                        OnPollingCycleCompleted(new PollingCycleCompletedEventArgs(changedItemsList, pollDuration, pollMonitoredItemsStarted));
                    }
                    Interlocked.Exchange(ref _isPolling, 0);
                    if (_pollingTimer != null && !_isDisposed)
                    {
                        try
                        {
                            if (pollDuration > PollingInterval)
                            {
                                _consecutiveSlowPolls++;
                                var backoffMultiplier = Math.Min(_consecutiveSlowPolls, MaxBackoffMultiplier);
                                var backoffDelay = PollingInterval * backoffMultiplier;
                                
                                _logger?.LogWarning($"{nameof(ApiPlcProgramSubscriptionFaker)}: Poll #{_consecutiveSlowPolls} exceeded interval. " +
                                    $"Expected: {PollingInterval}ms, Actual: {pollDuration:F0}ms. " +
                                    $"Applying exponential backoff (multiplier: {backoffMultiplier}x, delay: {backoffDelay}ms)");
                                
                                _pollingTimer.Change(backoffDelay, Timeout.Infinite);
                            }
                            else
                            {
                                if (_consecutiveSlowPolls > 0)
                                {
                                    _logger?.LogInformation($"{nameof(ApiPlcProgramSubscriptionFaker)}: Poll completed within interval after {_consecutiveSlowPolls} slow poll(s). Resetting backoff.");
                                    _consecutiveSlowPolls = 0;
                                }
                                
                                var delayUntilNextPoll = Math.Max(0, PollingInterval - (int)pollDuration);
                                _pollingTimer.Change(delayUntilNextPoll, Timeout.Infinite);
                            }
                        }
                        catch (ObjectDisposedException)
                        {

                        }
                    }
                }
            }
        }

        private ApiPlcProgramData GetChildWithName(ApiPlcProgramData root, string varNameForMethods)
        {
            var rootVarNameForMethods = root.GetVarNameForMethods();
            if (varNameForMethods == rootVarNameForMethods)
            {
                return root;
            }
            foreach(var child in root.Children)
            {
                var result = GetChildWithName(child, varNameForMethods);
                if (result != null)
                    return result;
            }
            throw new InvalidOperationException($"Could not find any element with {nameof(varNameForMethods)}:'{varNameForMethods}' underneath: {root}!");
        }

        private async Task<List<(ApiPlcProgramData item, object oldValue, object newValue)>> PollMonitoredItemsAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            List<ApiPlcProgramData> itemsToRead;
            lock (_monitoredItems)
            {
                if (_monitoredItems.Count == 0)
                    return new List<(ApiPlcProgramData item, object oldValue, object newValue)>();
                itemsToRead = new List<ApiPlcProgramData>(_monitoredItems);
            }
            var bulkRequest = new List<(IApiRequest apiRequest, ApiPlcProgramData item)>();
            foreach (var item in itemsToRead)
            {
                if (!item.Datatype.IsSupportedByPlcProgramReadOrWrite())
                {
                    var requests = await _plcProgramHandler.PlcProgramReadStructByChildValuesBulkRequestAsync(item, ApiPlcDataRepresentation.Simple, cancellationToken);
                    foreach(var request in requests)
                    {
                        var searchedItemString = request.Params["var"].ToString();
                        var accordingElement = GetChildWithName(item, searchedItemString);
                        bulkRequest.Add((request, accordingElement));
                    }
                    
                }
                else if (item.ArrayElements != null && item.ArrayElements.Count != 0)
                {
                    foreach (var arrayElement in item.ArrayElements)
                    {
                        var requestToAdd = _requestFactory.GetApiPlcProgramReadRequest(arrayElement.GetVarNameForMethods(), ApiPlcDataRepresentation.Simple);
                        bulkRequest.Add((requestToAdd, arrayElement));                            
                    }
                }
                else if (item.Children?.Count == 0)
                {
                    var requestToAdd = _requestFactory.GetApiPlcProgramReadRequest(item.GetVarNameForMethods(), ApiPlcDataRepresentation.Simple);
                    bulkRequest.Add((requestToAdd, item));
                }
            }
            if(bulkRequest.Count > 0)
            {
                var bulkRequestsToSend = _requestFactory.GetApiBulkRequestWithUniqueIds(bulkRequest.Select(el => el.apiRequest)).ToList();
                var counter = 0;
                foreach(var bulkRequestToSend in bulkRequestsToSend)
                {
                    bulkRequest[counter] = (bulkRequestToSend, bulkRequest[counter].item);
                    counter++;
                }
                _logger?.LogDebug($"{nameof(ApiPlcProgramSubscriptionFaker)}: Sending bulk request with: {bulkRequest.Count} elements ({MonitoredItems.Count} monitored items)");
                var monitemValues = await _apiRequestHandler.ApiBulkAsync(bulkRequestsToSend, cancellationToken);
                _logger?.LogDebug($"{nameof(ApiPlcProgramSubscriptionFaker)}: Got response for: {bulkRequest.Count} elements ({MonitoredItems.Count} monitored items)");
                
                var changedItems = new List<(ApiPlcProgramData item, object oldValue, object newValue)>();
                
                foreach (var childval in monitemValues.SuccessfulResponses)
                {
                    var accordingTuple = bulkRequest.First(el => el.apiRequest.Id == childval.Id);
                    var childOrArrayElementWithVarString = accordingTuple.item;
                    _logger?.LogDebug($"Apply value '{childval.Result}' to '{childOrArrayElementWithVarString.GetVarNameForMethods()}'");
                    
                    if(childOrArrayElementWithVarString.Value != childval.Result)
                    {
                        var oldValue = childOrArrayElementWithVarString.Value;
                        var newValue = childval.Result;
                        changedItems.Add((childOrArrayElementWithVarString, oldValue, newValue));
                        childOrArrayElementWithVarString.Value = newValue;
                        OnMonitoredItemValueChanged(new MonitoredItemValueChangedEventArgs(childOrArrayElementWithVarString, oldValue, newValue));
                    }
                    else
                    {
                        childOrArrayElementWithVarString.Value = childval.Result;
                    }
                }
                if (changedItems.Count > 0)
                {
                    _logger?.LogDebug($"{nameof(ApiPlcProgramSubscriptionFaker)}: {changedItems.Count} item(s) changed during this polling cycle");
                }
                return changedItems;
            }
            else
            {
                throw new InvalidOperationException($"Do not have any items to be read but monitored item count is not 0!" +
                    $"{string.Join(Environment.NewLine, MonitoredItems)}");
            }
        }

        /// <summary>
        /// Raises the MonitoredItemValueChanged event.
        /// </summary>
        protected virtual void OnMonitoredItemValueChanged(MonitoredItemValueChangedEventArgs e)
        {
            MonitoredItemValueChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Raises the PollingCycleCompleted event.
        /// </summary>
        protected virtual void OnPollingCycleCompleted(PollingCycleCompletedEventArgs e)
        {
            PollingCycleCompleted?.Invoke(this, e);
        }

        /// <summary>
        /// Dispose the subscription faker and release all resources.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            Stop();
        }
    }
}
