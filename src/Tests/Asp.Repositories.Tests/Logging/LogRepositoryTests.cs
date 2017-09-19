using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Core.Data;
using Asp.Core.Domains;
using Asp.Data;
using Asp.Repositories.Logging;
using Moq;
using Xunit;

namespace Asp.Repositories.Tests.Logging
{
    public class LogRepositoryTests
    {
        private readonly Log _log1;
        private readonly Log _log2;
        private readonly Log _log3;
        private readonly Log _log4;
        private readonly IQueryable<Log> _logs;
        private readonly IDbContext _mockDbContext;
        
        public LogRepositoryTests()
        {
            _log1 = new Log
            {
                Id = 1,
                Application = "ASP",
                Logged = new DateTime(2017, 01, 01),
                Level = "Info",
                Message = "Info message"
            };
            _log2 = new Log
            {
                Id = 2,
                Application = "ASP",
                Logged = new DateTime(2017, 01, 02),
                Level = "Debug",
                Message = "Debug message"
            };
            _log3 = new Log
            {
                Id = 3,
                Application = "ASP",
                Logged = new DateTime(2017, 01, 03),
                Level = "Error",
                Message = "Error message"
            };
            _log4 = new Log
            {
                Id = 4,
                Application = "ASP",
                Logged = new DateTime(2017, 01, 04),
                Level = "Error",
                Message = "Error message",
                Exception = "Error exception"
            };
            _logs = new List<Log>
            {
                _log1,
                _log2,
                _log3,
                _log4,
            }.AsQueryable();

            var mockSet = MockHelper.GetMockDbSet(_logs);
            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(x => x.Logs).Returns(mockSet.Object);
            _mockDbContext = mockDbContext.Object;
        }

        [Fact]
        public async Task GetLogs_ReturnLogs()
        {
            // Arrange
            var sut = new LogRepository(_mockDbContext);

            // Act
            var logs = await sut.GetLogs(new LogPagedDataRequest());

            // Assert
            Assert.Equal(4, logs.Count);
            Assert.True(logs.Contains(_log1));
            Assert.True(logs.Contains(_log2));
            Assert.True(logs.Contains(_log3));
            Assert.True(logs.Contains(_log4));
        }

        [Fact]
        public async Task GetLogs_SearchByDateRange_Return1Log()
        {
            // Arrange
            var sut = new LogRepository(_mockDbContext);

            // Act
            var logs = await  sut.GetLogs(new LogPagedDataRequest
            {
                FromDate = new DateTime(2017, 01, 02),
                ToDate = new DateTime(2017, 01, 02)
            });

            // Assert
            Assert.Equal(1, logs.Count);
            Assert.Equal(_log2.Id, logs[0].Id);
        }

        [Fact]
        public async Task GetLogs_SearchByLevel_Return2Logs()
        {
            // Arrange
            var sut = new LogRepository(_mockDbContext);

            // Act
            var logs = await sut.GetLogs(new LogPagedDataRequest {Level = "Error"});

            // Assert
            Assert.Equal(2, logs.Count);
            Assert.Equal(_log4.Id, logs[0].Id);
            Assert.Equal(_log3.Id, logs[1].Id);
        }

        [Fact]
        public async Task GetLogs_SearchByMessage_Return1Log()
        {
            // Arrange
            var sut = new LogRepository(_mockDbContext);

            // Act
            var logs = await sut.GetLogs(new LogPagedDataRequest {Message = "Error message" });

            // Assert
            Assert.Equal(2, logs.Count);
        }

        [Fact]
        public async Task GetLogs_SearchByException_Return1Log()
        {
            // Arrange
            var sut = new LogRepository(_mockDbContext);

            // Act
            var logs = await sut.GetLogs(new LogPagedDataRequest {Exception = "Error exception" });

            // Assert
            Assert.Equal(1, logs.Count);
            Assert.Equal(_log4.Id, logs[0].Id);
        }
    }
}