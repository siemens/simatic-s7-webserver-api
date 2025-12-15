using NUnit.Framework;
using Siemens.Simatic.S7.Webserver.API.Services.HelperHandlers;
using System;
using System.Threading;

namespace Webserver.API.UnitTests
{
    public class WaitHandlerTests 
    {

        [Test]
        public void WaitHandler_CancellationToken_ShouldThrowOperationCanceledException()
        {
            // Arrange
            var waitHandler = new WaitHandler(TimeSpan.FromSeconds(30));
            using(var cancellationTokenSource = new CancellationTokenSource())
            {
                cancellationTokenSource.Cancel(); // Cancel the token immediately
                                                  // Act & Assert
                Assert.Throws<OperationCanceledException>(() =>
                {
                    waitHandler.ForTrue(() => true, "Condition should be true", cancellationTokenSource.Token);
                });
            }
        }


        [Test]
        public void WaitHandler_ThrowsAfterTime_IncludingErrorInfo()
        {
            // Arrange
            var waitHandler = new WaitHandler(TimeSpan.FromMilliseconds(40));
            // Act & Assert
            var exc = Assert.Throws<TimeoutException>(() =>
            {
                waitHandler.ForTrue(() =>
                {
                    throw new InvalidOperationException($"Test");
                });
            });
            Assert.That(exc.InnerException.Message, Contains.Substring("Test"));
            Assert.That(exc.InnerException is InvalidOperationException);
            Assert.That(exc.ToString(), Contains.Substring("Test"));
        }

        [Test]
        public void WaitHandler_WorksWhenReturningTrue()
        {
            // Arrange
            var waitHandler = new WaitHandler(TimeSpan.FromMilliseconds(10));
            // Act & Assert
            var ts = waitHandler.ForTrue(() =>
            {
                return true;
            });
            Assert.That(ts, Is.LessThan(TimeSpan.FromMilliseconds(100)));
        }

        [Test]
        public void WaitHandler_ThrowsAfterTime_SleepsWhenNecessary()
        {
            // Arrange
            var waitHandler = new WaitHandler(TimeSpan.FromMilliseconds(20), TimeSpan.FromMilliseconds(10));
            // Act & Assert
            var exc = Assert.Throws<TimeoutException>(() =>
            {
                waitHandler.ForTrue(() =>
                {
                    return false;
                });
            });
            Assert.That(exc.ToString(), Contains.Substring($"{TimeSpan.FromMilliseconds(20)}"));
            Assert.That(exc.ToString(), Contains.Substring($"{TimeSpan.FromMilliseconds(10)}"));
        }
    }
}

