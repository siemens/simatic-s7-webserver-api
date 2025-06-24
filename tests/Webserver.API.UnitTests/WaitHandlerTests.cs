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
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel(); // Cancel the token immediately
            // Act & Assert
            Assert.Throws<OperationCanceledException>(() =>
            {
                waitHandler.ForTrue(() => true, "Condition should be true", cancellationTokenSource.Token);
            });
        }
    }
}
