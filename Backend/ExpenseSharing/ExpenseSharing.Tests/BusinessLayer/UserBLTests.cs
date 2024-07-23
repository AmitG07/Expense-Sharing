using Xunit;
using Moq;
using BusinessLayer;
using BusinessObjectLayer;
using DataAccessLayer.Interface;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace ExpenseSharing.Tests.BusinessLayer
{
    public class UserBLTests
    {
        private readonly Mock<IUserDAL> _mockUserDAL;
        private readonly UserBL _userBL;

        public UserBLTests()
        {
            _mockUserDAL = new Mock<IUserDAL>();
            _userBL = new UserBL(_mockUserDAL.Object);
        }

        [Fact]
        public void Authenticate_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var email = "test@example.com";
            var password = "password";
            var expectedUser = new User { UserId = 1, EmailId = email, Password = password };
            _mockUserDAL.Setup(x => x.GetUserByEmailAndPassword(email, password)).Returns(expectedUser);

            // Act
            var result = _userBL.Authenticate(email, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.UserId, result.UserId);
        }

        [Fact]
        public void Authenticate_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var email = "invalid@example.com";
            var password = "invalidpassword";
            _mockUserDAL.Setup(x => x.GetUserByEmailAndPassword(email, password)).Returns((User)null);

            // Act
            var result = _userBL.Authenticate(email, password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetUserById_ExistingUser_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var expectedUser = new User { UserId = userId };
            _mockUserDAL.Setup(x => x.GetUserById(userId)).Returns(expectedUser);

            // Act
            var result = _userBL.GetUserById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.UserId, result.UserId);
        }

        [Fact]
        public void GetUserById_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var userId = 999; // Non-existing ID
            _mockUserDAL.Setup(x => x.GetUserById(userId)).Returns((User)null);

            // Act
            var result = _userBL.GetUserById(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetAllUsers_ReturnsListOfUsers()
        {
            // Arrange
            var users = new List<User>
            {
                new User { UserId = 1, EmailId = "test1@example.com", Password = "password1" },
                new User { UserId = 2, EmailId = "test2@example.com", Password = "password2" }
            };
            _mockUserDAL.Setup(x => x.GetAllUsers()).Returns(users);

            // Act
            var result = _userBL.GetAllUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(users.Count, result.Count());
        }

        [Fact]
        public void GetAllUsers_ReturnsEmptyList()
        {
            // Arrange
            _mockUserDAL.Setup(x => x.GetAllUsers()).Returns(new List<User>());

            // Act
            var result = _userBL.GetAllUsers();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingUser_ReturnsUser()
        {
            // Arrange
            var userId = 1;
            var expectedUser = new User { UserId = userId };
            _mockUserDAL.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync(expectedUser);

            // Act
            var result = await _userBL.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser.UserId, result.UserId);
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var userId = 999; // Non-existing ID
            _mockUserDAL.Setup(x => x.GetUserByIdAsync(userId)).ReturnsAsync((User)null);

            // Act
            var result = await _userBL.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserAsync_ValidUser_Success()
        {
            // Arrange
            var userToUpdate = new User { UserId = 1, EmailId = "updated@example.com", Password = "updatedpassword" };

            // Act
            await _userBL.UpdateUserAsync(userToUpdate);

            // Assert
            _mockUserDAL.Verify(x => x.UpdateUser(userToUpdate), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_NullUser_ThrowsException()
        {
            // Arrange
            User userToUpdate = null;

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(async () => await _userBL.UpdateUserAsync(userToUpdate));

            // Assert
            Assert.Equal("user", exception.ParamName);
            Assert.Contains("User object cannot be null", exception.Message);
        }
    }
}
