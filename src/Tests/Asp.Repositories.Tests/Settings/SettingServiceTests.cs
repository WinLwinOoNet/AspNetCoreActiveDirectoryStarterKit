using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Core.Domains;
using Asp.Data;
using Asp.Repositories.Settings;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;
using Xunit;

namespace Asp.Repositories.Tests.Settings
{
    public class SettingServiceTests
    {
        private readonly Setting _setting1;
        private readonly Setting _setting2;
        private readonly Setting _setting3;
        private readonly Setting _setting4;
        private readonly IQueryable<Setting> _settings;

        public SettingServiceTests()
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
        }

        [Fact]
        public void GetSettingById_ValidId_Return1Setting()
        {
            // Arrange
            var mockSet = NSubstituteHelper.CreateMockDbSet(_settings);
            var mockContext = Substitute.For<IDbContext>();
            mockContext.Settings.Returns(mockSet);
            var sut = new SettingRepository(mockContext, null);

            // Act
            var setting = sut.GetSettingById(2);

            // Assert
            Assert.Equal(_setting2, setting);
        }

        [Fact]
        public void GetSettingById_InvalidId_ReturnNull()
        {
            // Arrange
            var mockSet = NSubstituteHelper.CreateMockDbSet(_settings);
            var mockContext = Substitute.For<IDbContext>();
            mockContext.Settings.Returns(mockSet);
            var sut = new SettingRepository(mockContext, null);

            // Act
            var setting = sut.GetSettingById(100);

            // Assert
            Assert.Null(setting);
        }

        [Fact]
        public void GetSettings_RetrieveAllSettings()
        {
            // Arrange
            var mockSet = NSubstituteHelper.CreateMockDbSet(_settings);
            var mockContext = Substitute.For<IDbContext>();
            mockContext.Settings.Returns(mockSet);

            var mockCache = Substitute.For<IMemoryCache>();
            Setting anySetting;
            mockCache.TryGetValue(Arg.Any<object>(), out anySetting).Returns(false);

            var sut = new SettingRepository(mockContext, mockCache);

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
            var mockSet = NSubstituteHelper.CreateMockDbSet(_settings);
            var mockContext = Substitute.For<IDbContext>();
            mockContext.Settings.Returns(mockSet);

            var mockCache = Substitute.For<IMemoryCache>();
            Setting anySetting;
            mockCache.TryGetValue(Arg.Any<object>(), out anySetting).Returns(false);

            var sut = new SettingRepository(mockContext, mockCache);

            // Act
            var setting = sut.GetSettingByKey("text", "");

            // Assert
            Assert.Equal(setting, "Alpha Bravo Charlie");
            mockCache.Received(1).TryGetValue(Arg.Any<object>(), out anySetting);
        }

        [Fact]
        public void GetSettings_ReturnBooleanValue()
        {
            // Arrange
            var mockSet = NSubstituteHelper.CreateMockDbSet(_settings);
            var mockContext = Substitute.For<IDbContext>();
            mockContext.Settings.Returns(mockSet);

            var mockCache = Substitute.For<IMemoryCache>();
            Setting anySetting;
            mockCache.TryGetValue(Arg.Any<object>(), out anySetting).Returns(false);

            var sut = new SettingRepository(mockContext, mockCache);

            // Act
            var setting = sut.GetSettingByKey("boolean", "");

            // Assert
            Assert.Equal(setting, "true");
            mockCache.Received(1).TryGetValue(Arg.Any<object>(), out anySetting);
        }

        [Fact]
        public void GetSettings_ReturnIntegerValue()
        {
            // Arrange
            var mockSet = NSubstituteHelper.CreateMockDbSet(_settings);
            var mockContext = Substitute.For<IDbContext>();
            mockContext.Settings.Returns(mockSet);

            var mockCache = Substitute.For<IMemoryCache>();
            Setting anySetting;
            mockCache.TryGetValue(Arg.Any<object>(), out anySetting).Returns(false);
            
            var sut = new SettingRepository(mockContext, mockCache);

            // Act
            var setting = sut.GetSettingByKey("integer", "");

            // Assert
            Assert.Equal(setting, "123");
            mockCache.Received(1).TryGetValue(Arg.Any<object>(), out anySetting);
        }

        [Fact]
        public void GetSettings_ReturnDecimalValue()
        {
            // Arrange
            var mockSet = NSubstituteHelper.CreateMockDbSet(_settings);
            var mockContext = Substitute.For<IDbContext>();
            mockContext.Settings.Returns(mockSet);

            var mockCache = Substitute.For<IMemoryCache>();
            Setting anySetting;
            mockCache.TryGetValue(Arg.Any<object>(), out anySetting).Returns(false);

            var sut = new SettingRepository(mockContext, mockCache);

            // Act
            var setting = sut.GetSettingByKey("decimal", "");

            // Assert
            Assert.Equal(setting, "12.34");
            mockCache.Received(1).TryGetValue(Arg.Any<object>(), out anySetting);
        }

        [Fact]
        public void GetSettings_ClearCache_CallMemoryCacheRemoveOnce()
        {
            // Arrange
            var mockSet = NSubstituteHelper.CreateMockDbSet(_settings);
            var mockContext = Substitute.For<IDbContext>();
            mockContext.Settings.Returns(mockSet);

            var mockCache = Substitute.For<IMemoryCache>();
            mockCache.Remove(Arg.Any<object>());

            var sut = new SettingRepository(mockContext, mockCache);

            // Act
            sut.ClearCache();

            // Assert
            mockCache.Received(1).Remove(Arg.Any<object>());
        }

        [Fact]
        public async Task UpdateSettingAsync_CallUpdateFromDbSetAndSaveChangesAsync()
        {
            // Arrange
            var mockSet = NSubstituteHelper.CreateMockDbSet(_settings);
            var mockContext = Substitute.For<IDbContext>();
            mockContext.Settings.Returns(mockSet);

            var mockCache = Substitute.For<IMemoryCache>();
            mockCache.Remove(Arg.Any<object>());
            Setting anySetting;
            mockCache.TryGetValue(Arg.Any<object>(), out anySetting).Returns(false);

            var sut = new SettingRepository(mockContext, mockCache);

            // Act
            await sut.UpdateSettingAsync(_setting1);
            var setting = sut.GetSettingByKey("text", "");

            // Assert
            Assert.Equal(_setting1.Value, setting);
            mockSet.Received(1).Update(Arg.Any<Setting>());
            await mockContext.Received(1).SaveChangesAsync();
            mockCache.Received(1).Remove(Arg.Any<object>());
        }
    }
}