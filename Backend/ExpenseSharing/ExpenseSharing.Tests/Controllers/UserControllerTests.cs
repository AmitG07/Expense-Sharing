using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using BusinessLayer.Interface;
using BusinessObjectLayer;
using ExpenseSharing.Controllers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ExpenseSharing.Models;

namespace ExpenseSharing.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserBL> _mockUserBL;
        private readonly Mock<ILogger<UserController>> _mockLogger;
        private readonly UserController _userController;

        public UserControllerTests()
        {
            _mockUserBL = new Mock<IUserBL>();
            _mockLogger = new Mock<ILogger<UserController>>();
            _userController = new UserController(_mockUserBL.Object, _mockLogger.Object);
        }

        [Fact]
        public void Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var login = new UserLogin { Email = "test@example.com", Password = "password" };
            var validUser = new User { UserId = 1, EmailId = login.Email, Password = login.Password };
            _mockUserBL.Setup(x => x.Authenticate(login.Email, login.Password)).Returns(validUser);

            // Act
            var result = _userController.Login(login);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userModel = Assert.IsType<User>(okResult.Value);
            Assert.Equal(validUser.UserId, userModel.UserId);
        }

        /*[Fact]
        public async Task Login_InvalidCredentials_ReturnsBadRequest()
        {
            // Arrange
            var login = new UserLogin { Email = "invalid@example.com", Password = "invalidpassword" };
            _mockUserBL.Setup(x => x.Authenticate(login.Email, login.Password)).Returns((User)null);

            // Act
            var result = _userController.Login(login);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = badRequestResult.Value as SerializableError;

            Assert.NotNull(errorResponse); // Ensure the error response is not null

            // Check if there is an error message with key "message"
            Assert.True(errorResponse.ContainsKey("message")); // Ensure there's a message key

            // Since errorResponse is SerializableError, it should contain key-value pairs of errors
            var messages = (string[])errorResponse["message"];
            Assert.Contains("Invalid email or password.", messages); // Check for specific error message
        }*/

        [Fact]
        public void GetUserById_ExistingUser_ReturnsOkResult()
        {
            // Arrange
            var userId = 1;
            var expectedUser = new User { UserId = userId };
            _mockUserBL.Setup(x => x.GetUserById(userId)).Returns(expectedUser);

            // Act
            var result = _userController.GetUserById(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userModel = Assert.IsType<User>(okResult.Value);
            Assert.Equal(expectedUser.UserId, userModel.UserId);
        }

        [Fact]
        public void GetUserById_NonExistingUser_ReturnsNotFound()
        {
            // Arrange
            var userId = 999; // Non-existing ID
            _mockUserBL.Setup(x => x.GetUserById(userId)).Returns((User)null);

            // Act
            var result = _userController.GetUserById(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetAllUsers_ReturnsOkResult()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, EmailId = "test1@example.com", Password = "password1" },
                new User { UserId = 2, EmailId = "test2@example.com", Password = "password2" }
            };
            _mockUserBL.Setup(x => x.GetAllUsers()).Returns(users);

            // Act
            var result = _userController.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userModelList = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Equal(users.Count, userModelList.Count());
        }

        [Fact]
        public void GetAllUsers_EmptyList_ReturnsOkResult()
        {
            // Arrange
            _mockUserBL.Setup(x => x.GetAllUsers()).Returns(new List<User>());

            // Act
            var result = _userController.GetAllUsers();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userModelList = Assert.IsAssignableFrom<IEnumerable<User>>(okResult.Value);
            Assert.Empty(userModelList);
        }
    }
}
