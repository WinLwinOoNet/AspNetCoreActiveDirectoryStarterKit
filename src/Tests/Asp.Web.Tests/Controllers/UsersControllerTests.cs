using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Asp.Core;
using Asp.Core.Data;
using Asp.Core.Domains;
using Asp.Repositories.Roles;
using Asp.Repositories.Users;
using Asp.Web.Areas.Administration.Controllers;
using Asp.Web.Common;
using Asp.Web.Common.Mapper;
using Asp.Web.Common.Models.UserViewModels;
using Asp.Web.Common.Mvc.Alerts;
using AutoMapper;
using Kendo.Mvc;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Asp.Web.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly User _user1;
        private readonly User _user2;
        private readonly User _user3;
        private readonly IPagedList<User> _users;
        private IList<Role> _roles;
        private Mock<IDateTime> _mockDateTime;
        private Mock<IUserRepository> _mockUserRepository;
        private Mock<IRoleRepository> _mockRoleRepository;
        private readonly Mapper _mapper;

        public UsersControllerTests()
        {
            _user1 = new User
            {
                Id = 1,
                UserName = "johndoe",
                FirstName = "John",
                LastName = "Doe",
                IsActive = true,
                LastLoginDate = new DateTime(2017, 01, 01),
                CreatedBy = "Administrator",
                CreatedOn = new DateTime(2017, 01, 01),
                ModifiedBy = "Administrator",
                ModifiedOn = new DateTime(2017, 01, 01)
            };
            _user2 = new User
            {
                Id = 2,
                UserName = "janetdoe",
                FirstName = "John",
                LastName = "Doe",
                IsActive = true,
                LastLoginDate = new DateTime(2017, 01, 01),
                CreatedBy = "Administrator",
                CreatedOn = new DateTime(2017, 01, 02),
                ModifiedBy = "Administrator",
                ModifiedOn = new DateTime(2017, 01, 02),
                UserRoles = new List<UserRole>
                {
                    new UserRole {Role = new Role {Id = 1, Name = "Administrator"}}
                }
            };
            _user3 = new User
            {
                Id = 3,
                UserName = "123456789",
                FirstName = "Eric",
                LastName = "Newton",
                IsActive = false,
                LastLoginDate = new DateTime(2017, 01, 01),
                CreatedBy = "Administrator",
                CreatedOn = new DateTime(2017, 01, 02),
                ModifiedBy = "Administrator",
                ModifiedOn = new DateTime(2017, 01, 02)
            };
            _users = new PagedList<User>
            {
                _user1,
                _user2,
                _user3
            };

            _roles = new List<Role>
            {
                new Role {Id = 1, Name = "Role1"},
                new Role {Id = 2, Name = "Role2"}
            };

            _mockDateTime = new Mock<IDateTime>();
            _mockDateTime.Setup(x => x.Now).Returns(new DateTime(2017, 01, 01));

            _mockRoleRepository = new Mock<IRoleRepository>();
            _mockRoleRepository.Setup(x => x.GetAllRolesAsync()).ReturnsAsync(_roles);

            _mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile())));
        }

        [Fact]
        public void Index_RedirectToActionList()
        {
            // Arrange
            var sut = new UsersController(null, null, null, null, null, null);

            // Act
            IActionResult result = sut.Index();

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("List", redirectToActionResult.ActionName);
            Assert.False(redirectToActionResult.Permanent);
        }

        [Fact]
        public async Task List_Get_ReturnViewResultWithRoles()
        {
            // Arrange
            var sut = new UsersController(null, null, null, _mockRoleRepository.Object, null, null);
            // Act
            var result = await sut.List();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("List", viewResult.ViewName);

            var model = Assert.IsType<UserSearchViewModel>(viewResult.Model);
            Assert.Equal(3, model.AvailableRoles.Count);

            Assert.Equal("All", model.AvailableRoles[0].Text);
            Assert.Equal("", model.AvailableRoles[0].Value);

            Assert.Equal(_roles[0].Name, model.AvailableRoles[1].Text);
            Assert.Equal(_roles[0].Id.ToString(), model.AvailableRoles[1].Value);
        }

        [Fact]
        public async Task List_Post_NoFilter_ReturnJsonResult()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.GetUsersAsync(It.IsAny<UserPagedDataRequest>())).ReturnsAsync(_users);

            var sut = new UsersController(null, _mapper, null, _mockRoleRepository.Object, mockUserRepository.Object, null);

            var request = new DataSourceRequest {Sorts = new List<SortDescriptor>()};
            var model = new UserSearchViewModel();

            // Act
            var result = await sut.List(request, model);

            // Assert
            var jsonResult = Assert.IsType<JsonResult>(result);
            var dataSourceResult = Assert.IsType<DataSourceResult>(jsonResult.Value);
            var models = Assert.IsType<List<UserViewModel>>(dataSourceResult.Data);
            Assert.Equal(3, models.Count);
        }

        [Fact]
        public async Task Create_Get_ReturnViewResultWithRoles()
        {
            // Arrange
            var sut = new UsersController(null, null, null, _mockRoleRepository.Object, null, null);

            // Act
            var result = await sut.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", viewResult.ViewName);

            var model = Assert.IsType<UserCreateUpdateViewModel>(viewResult.Model);
            Assert.Equal(2, model.AvailableRoles.Count);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_NotSaveUser_ReturnViewResult()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            var sut = new UsersController(null, null, null, _mockRoleRepository.Object, null, null);
            sut.ModelState.AddModelError("x", "Test Error");

            var expectedModel = new UserCreateUpdateViewModel {UserName = "johndoe"};

            // Act
            var result = await sut.Create(expectedModel);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Create", viewResult.ViewName);

            var model = Assert.IsType<UserCreateUpdateViewModel>(viewResult.Model);
            Assert.Equal(expectedModel.UserName, model.UserName);
            Assert.Equal(2, model.AvailableRoles.Count);
            mockUserRepository.Verify(x => x.AddUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Create_Post_UserAlreadyExists_NotSaveUser_RedirectToListWithErrorMessage()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.GetUserByUserNameAsync(It.IsAny<string>())).ReturnsAsync(_user1);

            var sut = new UsersController(null, null, null, _mockRoleRepository.Object,
                mockUserRepository.Object, null);

            var expectedModel = new UserCreateUpdateViewModel {UserName = _user1.UserName};

            // Act
            var result = await sut.Create(expectedModel);

            // Assert
            mockUserRepository.Verify(x => x.GetUserByUserNameAsync(It.IsAny<string>()), Times.Once);
            mockUserRepository.Verify(x => x.AddUserAsync(It.IsAny<User>()), Times.Never);
            _mockRoleRepository.Verify(x => x.GetAllRolesAsync(), Times.Never);

            var alertDecoratorResult = Assert.IsType<AlertDecoratorResult>(result);
            Assert.Equal(alertDecoratorResult.Message,
                $"User with same username {expectedModel.UserName} alredy exists.");

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(alertDecoratorResult.InnerResult);
            Assert.False(redirectToActionResult.Permanent);
            Assert.Equal(redirectToActionResult.ActionName, "List");
        }

        [Fact]
        public async Task Create_Post_ValidUser_SaveUserAndRedirectToListWithSuccessfulMessage()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.GetUserByUserNameAsync(It.IsAny<string>())).ReturnsAsync((User) null);
            mockUserRepository.Setup(x => x.AddUserAsync(It.IsAny<User>())).ReturnsAsync(1);

            var mockUserSession = new Mock<IUserSession>();
            mockUserSession.Setup(x => x.UserName).Returns("johndoe");

            var sut = new UsersController(_mockDateTime.Object, _mapper, null, _mockRoleRepository.Object,
                mockUserRepository.Object, mockUserSession.Object);

            var model = new UserCreateUpdateViewModel {UserName = _user1.UserName, FirstName = _user1.FirstName};

            // Act
            var result = await sut.Create(model);

            // Assert
            mockUserRepository.Verify(x => x.GetUserByUserNameAsync(It.IsAny<string>()), Times.Once);
            _mockDateTime.Verify(x => x.Now, Times.Once);
            _mockRoleRepository.Verify(x => x.GetAllRolesAsync(), Times.Once);
            mockUserRepository.Verify(x => x.AddUserAsync(It.IsAny<User>()), Times.Once);

            var alertDecoratorResult = Assert.IsType<AlertDecoratorResult>(result);
            Assert.Equal(alertDecoratorResult.Message, $"{_user1.FirstName}'s account was created successfully.");

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(alertDecoratorResult.InnerResult);
            Assert.False(redirectToActionResult.Permanent);
            Assert.Equal("List", redirectToActionResult.ActionName);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(-1)]
        [InlineData(100)]
        public async Task Edit_Get_InvalidId_RedirectToList(int? id)
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((User) null);

            var sut = new UsersController(null, null, null, null, mockUserRepository.Object, null);

            // Act
            var result = await sut.Edit(id);

            // Assert
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.False(redirectToActionResult.Permanent);
            Assert.Equal("List", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Edit_Get_ValidId_ReturnViewResultWithRoles()
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.GetUserByIdAsync(_user1.Id)).ReturnsAsync(_user1);
            
            var mockRoleRepository = new Mock<IRoleRepository>();
            mockRoleRepository.Setup(x => x.GetAllRolesAsync()).ReturnsAsync(_roles);
            mockRoleRepository.Setup(x => x.GetUserRolesForUserAsync(_user1.Id)).ReturnsAsync(new List<UserRole>());

            var sut = new UsersController(null, _mapper, null, mockRoleRepository.Object, mockUserRepository.Object, null);

            // Act
            var result = await sut.Edit(_user1.Id);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Edit", viewResult.ViewName);

            var model = Assert.IsType<UserCreateUpdateViewModel>(viewResult.Model);
            Assert.Equal(2, model.AvailableRoles.Count);
            
            Assert.Equal(_roles[0].Name, model.AvailableRoles[0].Text);
            Assert.Equal(_roles[0].Id.ToString(), model.AvailableRoles[0].Value);

            mockRoleRepository.Verify(x => x.GetAllRolesAsync(), Times.Once);
            mockRoleRepository.Verify(x => x.GetUserRolesForUserAsync(It.IsAny<int>()), Times.Once);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(100)]
        public async Task Edit_Post_InvalidId_NotSaveUserAndRedirectToListWithErrorMessage(int id)
        {
            // Arrange
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync((User) null);

            var sut = new UsersController(null, null, null, null, mockUserRepository.Object, null);

            var model = new UserCreateUpdateViewModel {Id = id, UserName = _user1.UserName};

            // Act
            var result = await sut.Edit(model);

            // Assert
            mockUserRepository.Verify(x => x.GetUserByIdAsync(It.IsAny<int>()), Times.Once);
            mockUserRepository.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Never);

            var alertDecoratorResult = Assert.IsType<AlertDecoratorResult>(result);
            Assert.Equal(alertDecoratorResult.Message, "Please select a user.");

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(alertDecoratorResult.InnerResult);
            Assert.False(redirectToActionResult.Permanent);
            Assert.Equal("List", redirectToActionResult.ActionName);
        }

        [Fact]
        public async Task Edit_Post_ValidId_SaveUserAndRedirectToListWithSuccessfulMessage()
        {
            // Arrange
            User savedUser = null;
            var mockUserRepository = new Mock<IUserRepository>();
            mockUserRepository.Setup(x => x.GetUserByIdAsync(_user1.Id)).ReturnsAsync(_user1);
            mockUserRepository.Setup(x => x.UpdateUserAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask)
                .Callback<User>(x => savedUser = x);

            var mockRoleRepository = new Mock<IRoleRepository>();
            mockRoleRepository.Setup(x => x.GetAllRolesAsync()).ReturnsAsync(_roles);
            mockRoleRepository.Setup(x => x.GetUserRolesForUserAsync(_user1.Id)).ReturnsAsync(new List<UserRole>());

            var mockUserSession = new Mock<IUserSession>();
            mockUserSession.Setup(x => x.UserName).Returns("johndoe");

            var sut = new UsersController(_mockDateTime.Object, _mapper, null, mockRoleRepository.Object,
                mockUserRepository.Object, mockUserSession.Object);

            var model = new UserCreateUpdateViewModel
            {
                Id = _user1.Id,
                UserName = _user1.UserName,
                FirstName = _user1.FirstName,
                SelectedRoleIds = new List<int> {1}
            };

            // Act
            var result = await sut.Edit(model);

            // Assert
            mockUserRepository.Verify(x => x.GetUserByIdAsync(It.IsAny<int>()), Times.Once);
            _mockDateTime.Verify(x => x.Now, Times.Once);
            mockUserSession.Verify(x => x.UserName, Times.Once);
            mockRoleRepository.Verify(x => x.GetAllRolesAsync(), Times.Once);
            mockRoleRepository.Verify(x => x.GetUserRolesForUserAsync(It.IsAny<int>()), Times.Once);
            mockUserRepository.Verify(x => x.UpdateUserAsync(It.IsAny<User>()), Times.Once);

            Assert.Equal(model.Id, savedUser.Id);
            Assert.Equal(model.UserName, savedUser.UserName);
            Assert.Equal(model.FirstName, savedUser.FirstName);

            var alertDecoratorResult = Assert.IsType<AlertDecoratorResult>(result);
            Assert.Equal(alertDecoratorResult.Message, $"{_user1.FirstName}'s account was updated successfully.");

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(alertDecoratorResult.InnerResult);
            Assert.False(redirectToActionResult.Permanent);
            Assert.Equal("List", redirectToActionResult.ActionName);
        }
    }
}