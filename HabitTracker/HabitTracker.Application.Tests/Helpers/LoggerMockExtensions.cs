using Microsoft.Extensions.Logging;
using Moq;
using System;

namespace HabitTracker.Application.Tests.Helpers
{
    public static class LoggerMockExtensions
    {
        public static void VerifyLog<T>(
            this Mock<ILogger<T>> loggerMock,
            LogLevel level,
            string containsMessage,
            Times times)
        {
            loggerMock.Verify(
                x => x.Log(
                    level,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains(containsMessage)),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()
                ),
                times
            );
        }

        public static void VerifyLogError<T>(
            this Mock<ILogger<T>> loggerMock,
            string containsMessage,
            Times times)
        {
            loggerMock.VerifyLog(LogLevel.Error, containsMessage, times);
        }
    }
}
