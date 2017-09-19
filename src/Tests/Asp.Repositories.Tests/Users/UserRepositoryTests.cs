using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Asp.Core.Domains;
using Asp.Data;
using Asp.Repositories.Users;
using Moq;
using Xunit;

namespace Asp.Repositories.Tests.Users
{
    public class UserRepositoryTests
    {
        private readonly User _user1;
        private readonly User _user2;
        private readonly User _user3;
        private readonly IQueryable<User> _users;
        private readonly IDbContext _mockDbContext;

        public UserRepositoryTests()
        {
            _user1 = new User
            {
                Id = 1,
                UserName = "johndoe",
                FirstName = "John",
                LastName = "Doe",
                IsActive = true,
                LastLoginDate = new DateTime(2017, 01, 01),
                CreatedBy = "Developer",
                CreatedOn = new DateTime(2017, 01, 01),
                ModifiedBy = "Developer",
                ModifiedOn = new DateTime(2017, 01, 02)
            };
            _user2 = new User
            {
                Id = 2,
                UserName = "janetdoe",
                FirstName = "John",
                LastName = "Doe",
                IsActive = true,
                LastLoginDate = new DateTime(2017, 01, 01),
                CreatedBy = "Developer",
                CreatedOn = new DateTime(2017, 01, 02),
                ModifiedBy = "Developer",
                ModifiedOn = new DateTime(2017, 01, 02)
            };
            _user3 = new User
            {
                Id = 3,
                UserName = "123456789",
                FirstName = "Eric",
                LastName = "Newton",
                IsActive = false,
                LastLoginDate = new DateTime(2017, 01, 01),
                CreatedBy = "Developer",
                CreatedOn = new DateTime(2017, 01, 02),
                ModifiedBy = "Developer",
                ModifiedOn = new DateTime(2017, 01, 02)
            };
            _users = new List<User>
            {
                _user1,
                _user2,
                _user3
            }.AsQueryable();

            var mockSet = MockHelper.GetMockDbSet(_users);
            var mockDbContext = new Mock<IDbContext>();
            mockDbContext.Setup(x => x.Users).Returns(mockSet.Object);
            _mockDbContext = mockDbContext.Object;
        }

        [Fact]
        public async Task GetUserByIdAsync_ValidId_Return1User()
        {
            // Arrange
            var sut = new UserRepository(_mockDbContext);

            // Act
            var user = await sut.GetUserByIdAsync(_user2.Id);

            // Assert
            Assert.Equal(_user2, user);
        }

        [Fact]
        public async Task GetUserByIdAsync_InvalidId_ReturnNull()
        {
            // Arrange
            var sut = new UserRepository(_mockDbContext);

            // Act
            var user = await sut.GetUserByIdAsync(0);

            // Assert
            Assert.Null(user);
        }

        [Fact]
        public async Task GetUserByUserNameAsync_ValidUserName_Return1User()
        {
            // Arrange
            var sut = new UserRepository(_mockDbContext);

            // Act
            var user = await sut.GetUserByUserNameAsync(_user2.UserName);

            // Assert
            Assert.Equal(_user2, user);
        }

        [Fact]
        public async Task GetUserByUserNameAsync_EmptyUserName_ThrowArgumentNullException()
        {
            // Arrange
            var sut = new UserRepository(_mockDbContext);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => sut.GetUserByUserNameAsync(""));
        }

        [Fact]
        public async Task GetUserByUserNameAsync_InvalidUserName_ReturnNull()
        {
            // Arrange
            var sut = new UserRepository(_mockDbContext);

            // Act
            var user = await sut.GetUserByUserNameAsync("donotexist");

            // Assert
            Assert.Null(user);
        }
    }
}