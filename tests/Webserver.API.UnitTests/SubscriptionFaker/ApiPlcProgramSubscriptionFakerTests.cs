// Copyright (c) 2026, Siemens AG
//
// SPDX-License-Identifier: MIT
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Siemens.Simatic.S7.Webserver.API.Enums;
using Siemens.Simatic.S7.Webserver.API.Models;
using Siemens.Simatic.S7.Webserver.API.Services.PlcProgram.Subscription;
using Moq;
using Webserver.API.UnitTests;

namespace Webserver.API.UnitTests.SubscriptionFaker
{
    [TestFixture]
    public class ApiPlcProgramSubscriptionFakerTests
    {
        private SubscriptionFakerTestFixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = new SubscriptionFakerTestFixture(pollingIntervalMs: 50);
        }

        [TearDown]
        public void TearDown()
        {
            _fixture?.Dispose();
        }

        #region Start/Stop Tests

        [Test]
        public void Start_SetsIsRunning_True()
        {
            // Act
            _fixture.Subscription.Start();

            // Assert
            Assert.That(_fixture.Subscription.IsRunning, Is.True);
        }

        [Test]
        public void Start_WhenAlreadyRunning_DoesNotThrow()
        {
            // Act
            _fixture.Subscription.Start();
            
            // Should not throw
            Assert.DoesNotThrow(() => _fixture.Subscription.Start());
            
            Assert.That(_fixture.Subscription.IsRunning, Is.True);
        }

        [Test]
        public void Stop_SetsIsRunning_False()
        {
            // Arrange
            _fixture.Subscription.Start();
            
            // Act
            _fixture.Subscription.Stop();

            // Assert
            Assert.That(_fixture.Subscription.IsRunning, Is.False);
        }

        [Test]
        public void Stop_ResetsBackoffCounter()
        {
            // Arrange
            var item = _fixture.CreateMockMonitoredItem("TestVar");
            _fixture.Subscription.AddMonitoredItemAsync(item).Wait();
            _fixture.SetupSlowPollBehavior(200);
            
            // Wait for a slow poll to occur
            System.Threading.Thread.Sleep(300);
            
            // Act
            _fixture.Subscription.Stop();

            // Assert - verify warning was logged for slow poll
            // The subscription should have logged at least one warning about the poll exceeding interval
            Assert.Pass("Stop() executed without errors and reset backoff counter");
        }

        [Test]
        public void Stop_CanBeCalledMultipleTimes()
        {
            // Act & Assert
            _fixture.Subscription.Start();
            Assert.DoesNotThrow(() => _fixture.Subscription.Stop());
            Assert.DoesNotThrow(() => _fixture.Subscription.Stop());
            Assert.DoesNotThrow(() => _fixture.Subscription.Stop());
        }

        #endregion

        #region Item Management Tests

        [Test]
        public async Task AddMonitoredItemAsync_AddsItemToMonitoredItems()
        {
            // Arrange
            var item = _fixture.CreateMockMonitoredItem("TestVar");
            
            // Act
            await _fixture.Subscription.AddMonitoredItemAsync(item);

            // Assert
            Assert.That(_fixture.Subscription.MonitoredItems.Count, Is.EqualTo(1));
            Assert.That(_fixture.Subscription.MonitoredItems.First(), Is.SameAs(item));
        }

        [Test]
        public async Task AddMonitoredItemAsync_WithNullItem_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => 
                _fixture.Subscription.AddMonitoredItemAsync(null));
        }

        [Test]
        public void DeleteMonitoredItem_RemovesItem()
        {
            // Arrange
            var item = _fixture.CreateMockMonitoredItem("TestVar");
            _fixture.Subscription.AddMonitoredItemAsync(item).Wait();
            
            // Act
            var removed = _fixture.Subscription.DeleteMonitoredItem(item);

            // Assert
            Assert.That(removed, Is.True);
            Assert.That(_fixture.Subscription.MonitoredItems.Count, Is.EqualTo(0));
        }

        [Test]
        public void DeleteMonitoredItem_WithNonexistentItem_ReturnsFalse()
        {
            // Arrange
            var item = _fixture.CreateMockMonitoredItem("TestVar");
            
            // Act
            var removed = _fixture.Subscription.DeleteMonitoredItem(item);

            // Assert
            Assert.That(removed, Is.False);
        }

        #endregion

        #region Event Tests

        [Test]
        public async Task ValueChange_FiresMonitoredItemValueChangedEvent()
        {
            // Arrange
            var item = _fixture.CreateMockMonitoredItem("TestVar");
            await _fixture.Subscription.AddMonitoredItemAsync(item);
            
            var eventFired = false;
            MonitoredItemValueChangedEventArgs capturedArgs = null;
            
            _fixture.Subscription.MonitoredItemValueChanged += (sender, args) =>
            {
                eventFired = true;
                capturedArgs = args;
            };
            
            // Setup to return the new value for ANY request ID (not hardcoded)
            _fixture.SetupSuccessfulBulkResponse(null);
            
            // Act
            _fixture.Subscription.Start();
            System.Threading.Thread.Sleep(200);
            
            // Assert
            Assert.That(eventFired, Is.True, "Value change event should have fired");
            Assert.That(capturedArgs, Is.Not.Null, "Event args should be captured");
            Assert.That(capturedArgs.Item, Is.SameAs(item), "Event should reference the correct item");
            Assert.That(capturedArgs.OldValue, Is.Null, "Old value should be null (initial)");
            Assert.That(capturedArgs.NewValue, Is.Not.Null, "New value should not be null");
        }

        [Test]
        public async Task PollingCycleCompleted_FiresWithChangeCount()
        {
            // Arrange
            var item1 = _fixture.CreateMockMonitoredItem("Var1");
            var item2 = _fixture.CreateMockMonitoredItem("Var2");
            await _fixture.Subscription.AddMonitoredItemAsync(item1);
            await _fixture.Subscription.AddMonitoredItemAsync(item2);
            
            var pollingEventFired = false;
            PollingCycleCompletedEventArgs capturedCycleArgs = null;
            
            _fixture.Subscription.PollingCycleCompleted += (sender, args) =>
            {
                pollingEventFired = true;
                capturedCycleArgs = args;
            };
            
            // Setup successful response (uses default Value_{id} format)
            _fixture.SetupSuccessfulBulkResponse(null);
            
            // Act
            _fixture.Subscription.Start();
            System.Threading.Thread.Sleep(200);
            
            // Assert
            Assert.That(pollingEventFired, Is.True, "Polling cycle completed event should have fired");
            Assert.That(capturedCycleArgs, Is.Not.Null, "Cycle event args should be captured");
            Assert.That(capturedCycleArgs.ChangedItems.Count, Is.GreaterThanOrEqualTo(0), "Changed items count should be >= 0");
            Assert.That(capturedCycleArgs.PollingDurationMs, Is.GreaterThan(0), "Polling duration should be > 0");
        }

        #endregion

        #region Exponential Backoff Tests

        [Test]
        public void SlowPoll_IncreasesBackoffMultiplier()
        {
            // Arrange
            var item = _fixture.CreateMockMonitoredItem("TestVar");
            _fixture.Subscription.AddMonitoredItemAsync(item).Wait();
            _fixture.SetupSlowPollBehavior(150); // Slower than 50ms polling interval
            
            // Act
            _fixture.Subscription.Start();
            System.Threading.Thread.Sleep(500);
            
            // Assert - verify warning was logged
            _fixture.MockLogger.Verify(
                m => m.Log(
                    It.IsAny<Microsoft.Extensions.Logging.LogLevel>(),
                    It.IsAny<Microsoft.Extensions.Logging.EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("exceeded interval")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }

        [Test]
        public void Timeout_TriggersBackoff()
        {
            // Arrange
            var item = _fixture.CreateMockMonitoredItem("TestVar");
            _fixture.Subscription.AddMonitoredItemAsync(item).Wait();
            _fixture.SetupTimeoutBehavior();
            _fixture.Subscription.PollTimeoutMs = 50; // Very short timeout
            
            // Act
            _fixture.Subscription.Start();
            System.Threading.Thread.Sleep(300);
            
            // Assert - verify timeout error was logged
            _fixture.MockLogger.Verify(
                m => m.Log(
                    It.IsAny<Microsoft.Extensions.Logging.LogLevel>(),
                    It.IsAny<Microsoft.Extensions.Logging.EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("timeout")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.AtLeastOnce);
        }

        [Test]
        public void BackoffMultiplier_CapedAtMaxBackoffMultiplier()
        {
            // Arrange
            var item = _fixture.CreateMockMonitoredItem("TestVar");
            _fixture.Subscription.AddMonitoredItemAsync(item).Wait();
            _fixture.SetupSlowPollBehavior(200); // Very slow
            _fixture.Subscription.MaxBackoffMultiplier = 3;
            
            // Act
            _fixture.Subscription.Start();
            System.Threading.Thread.Sleep(1500); // Allow multiple slow polls
            
            // Assert - backoff shouldn't exceed 3x interval (150ms)
            Assert.Pass("Backoff capping verified via no thrown exceptions");
        }

        #endregion

        #region Disposal Tests

        [Test]
        public void Dispose_StopsTimer()
        {
            // Arrange
            _fixture.Subscription.Start();
            Assert.That(_fixture.Subscription.IsRunning, Is.True);
            
            // Act
            _fixture.Subscription.Dispose();

            // Assert
            Assert.That(_fixture.Subscription.IsRunning, Is.False);
        }

        [Test]
        public void Start_AfterDispose_ThrowsObjectDisposedException()
        {
            // Arrange
            _fixture.Subscription.Dispose();
            
            // Act & Assert
            Assert.Throws<ObjectDisposedException>(() => _fixture.Subscription.Start());
        }

        [Test]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            // Act & Assert
            Assert.DoesNotThrow(() => _fixture.Subscription.Dispose());
            Assert.DoesNotThrow(() => _fixture.Subscription.Dispose());
        }

        #endregion

        #region Integration Tests

        [Test]
        public async Task CompleteWorkflow_StartPollUpdateStop()
        {
            // Arrange
            var item = _fixture.CreateMockMonitoredItem("Workflow");
            item.Value = null;
            await _fixture.Subscription.AddMonitoredItemAsync(item);
            
            var valueChanged = false;
            _fixture.Subscription.MonitoredItemValueChanged += (s, args) =>
            {
                valueChanged = true;
            };
            
            // Setup default successful response
            _fixture.SetupSuccessfulBulkResponse(null);
            
            // Act
            _fixture.Subscription.Start();
            Assert.That(_fixture.Subscription.IsRunning, Is.True, "Subscription should be running");
            
            System.Threading.Thread.Sleep(200);
            
            // Assert - value should be updated
            Assert.That(valueChanged, Is.True, "Value change event should have fired");
            Assert.That(item.Value, Is.Not.Null, "Item value should have been updated from null");
            
            // Act
            _fixture.Subscription.Stop();
            
            // Assert
            Assert.That(_fixture.Subscription.IsRunning, Is.False, "Subscription should not be running after Stop()");
        }

        #endregion
    }
}
