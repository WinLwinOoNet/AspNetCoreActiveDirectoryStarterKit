using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Asp.Core.Domains;
using Asp.Data;
using Asp.Repositories.Settings;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace Asp.Repositories.Tests.Settings
{
    public class SettingRepositoryTests
    {
        private readonly Setting _setting1;
        private readonly Setting _setting2;
        private readonly Setting _setting3;
        private readonly Setting _setting4;
        private readonly IQueryable<Setting> _settings;
        private readonly IDbContext _mockDbContext;

        public SettingRepositoryTests()
        {
            _setting1 = new Setting
            {
                Id = 1,
                Name = "text",
                Value = "Alpha Bravo Charlie"
            };
            _setting2 = new Setting
            {
                Id = 2,
                Name = "boolean",
                Value = "true"
            };
            _setting3 = new Setting
            {
                Id = 3,
                Name = "integer",
                Value = "123"
            };
            _setting4 = new Setting
            {
                Id = 4,
                Name = "decimal",
                Value = "12.34"
            };
            _settings = new List<Setting>
            {
                _setting1,
                _setting2,
                _setting3,
                _setting4,
            }.AsQueryable();

            var mockSet = MockHelper.GetMockDbSet(_settings);
            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(x => x.Settings).Returns(mockSet.Object);
            _mockDbContext = mockDbContext.Object;
        }

        [Fact]
        public void GetSettingById_ValidId_Return1Setting()
        {
            // Arrange
            var sut = new SettingRepository(_mockDbContext, null);

            // Act
            var setting = sut.GetSettingById(2);

            // Assert
            Assert.Equal(_setting2, setting);
        }

        [Fact]
        public void GetSettingById_InvalidId_ReturnNull()
        {
            // Arrange
            var sut = new SettingRepository(_mockDbContext, null);

            // Act
            var setting = sut.GetSettingById(100);

            // Assert
            Assert.Null(setting);
        }

        [Fact]
        public void GetSettings_RetrieveAllSettings()
        {
            // Arrange
            var fakeMemoryCache = new FakeMemoryCache();
            var sut = new SettingRepository(_mockDbContext, fakeMemoryCache);

            // Act
            var settings = sut.GetAllSettings();

            // Assert
            Assert.Equal(4, settings.Count);
            Assert.True(settings.Contains(_setting1));
            Assert.True(settings.Contains(_setting2));
            Assert.True(settings.Contains(_setting3));
            Assert.True(settings.Contains(_setting4));
        }

        [Fact]
        public void GetSettings_ReturnTextValue()
        {
            // Arrange
            var fakeMemoryCache = new FakeMemoryCache();
            var sut = new SettingRepository(_mockDbContext, fakeMemoryCache);

            // Act
            var setting = sut.GetSettingByKey("text", "");

            // Assert
            Assert.Equal(setting, "Alpha Bravo Charlie");
        }

        [Fact]
        public void GetSettings_ReturnBooleanValue()
        {
            // Arrange
            var fakeMemoryCache = new FakeMemoryCache();
            var sut = new SettingRepository(_mockDbContext, fakeMemoryCache);

            // Act
            var setting = sut.GetSettingByKey("boolean", "");

            // Assert
            Assert.Equal(setting, "true");
        }

        [Fact]
        public void GetSettings_ReturnIntegerValue()
        {
            // Arrange
            var fakeMemoryCache = new FakeMemoryCache();
            var sut = new SettingRepository(_mockDbContext, fakeMemoryCache);

            // Act
            var setting = sut.GetSettingByKey("integer", "");

            // Assert
            Assert.Equal(setting, "123");
        }

        [Fact]
        public void GetSettings_ReturnDecimalValue()
        {
            // Arrange
            var fakeMemoryCache = new FakeMemoryCache();
            var sut = new SettingRepository(_mockDbContext, fakeMemoryCache);

            // Act
            var setting = sut.GetSettingByKey("decimal", "");

            // Assert
            Assert.Equal(setting, "12.34");
        }

        /*[Fact]
        public async Task UpdateSettingAsync_CallUpdateFromDbSetAndSaveChangesAsync()
        {
            // Arrange
            var mockSet = MockHelper.GetMockDbSet(_settings);
            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(x => x.Settings).Returns(mockSet.Object);
            mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(_setting1.Id);

            var fakeMemoryCache = new FakeMemoryCache();
            var sut = new SettingRepository(_mockDbContext, fakeMemoryCache);

            // Act
            await sut.UpdateSettingAsync(_setting1);
            var setting = sut.GetSettingByKey("text", "");

            // Assert
            Assert.Equal(_setting1.Value, setting);
            mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }*/
    }
}