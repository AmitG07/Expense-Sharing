using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccessLayer;
using DataAccessLayer.Interface;
using BusinessObjectLayer;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace ExpenseSharing.Tests.DataAccessLayer
{
    public class UserDALTests
    {
        [Fact]
        public void GetUserByEmailAndPassword_ValidCredentials_ReturnsUser()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Users.AddRange(
                    new User { UserId = 1, EmailId = "test@example.com", Password = "password" },
                    new User { UserId = 2, EmailId = "admin@example.com", Password = "adminpassword" }
                );
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var userDAL = new UserDAL(context);

                // Act
                var user = userDAL.GetUserByEmailAndPassword("test@example.com", "password");

                // Assert
                Assert.NotNull(user);
                Assert.Equal(1, user.UserId);
            }
        }

        [Fact]
        public void GetUserByEmailAndPassword_InvalidCredentials_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Users.AddRange(
                    new User { UserId = 1, EmailId = "test@example.com", Password = "password" },
                    new User { UserId = 2, EmailId = "admin@example.com", Password = "adminpassword" }
                );
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var userDAL = new UserDAL(context);

                // Act
                var user = userDAL.GetUserByEmailAndPassword("invalid@example.com", "invalidpassword");

                // Assert
                Assert.Null(user);
            }
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingUser_ReturnsUser()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Users.AddRange(
                    new User { UserId = 1, EmailId = "test@example.com", Password = "password" },
                    new User { UserId = 2, EmailId = "admin@example.com", Password = "adminpassword" }
                );
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var userDAL = new UserDAL(context);

                // Act
                var user = await userDAL.GetUserByIdAsync(1);

                // Assert
                Assert.NotNull(user);
                Assert.Equal(1, user.UserId);
            }
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Users.AddRange(
                    new User { UserId = 1, EmailId = "test@example.com", Password = "password" },
                    new User { UserId = 2, EmailId = "admin@example.com", Password = "adminpassword" }
                );
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var userDAL = new UserDAL(context);

                // Act
                var user = await userDAL.GetUserByIdAsync(999); // Non-existing ID

                // Assert
                Assert.Null(user);
            }
        }

        [Fact]
        public async Task UpdateUser_ValidUser_UpdatesSuccessfully()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var userToUpdate = new User { UserId = 1, EmailId = "test@example.com", Password = "password" };

            using (var context = new ApplicationDbContext(options))
            {
                context.Users.Add(userToUpdate);
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var userDAL = new UserDAL(context);

                // Act
                userToUpdate.EmailId = "updated@example.com";
                await userDAL.UpdateUser(userToUpdate);

                // Retrieve updated user
                var updatedUser = await userDAL.GetUserByIdAsync(1);

                // Assert
                Assert.NotNull(updatedUser);
                Assert.Equal("updated@example.com", updatedUser.EmailId);
            }
        }

        [Fact]
        public async Task UpdateUser_NonExistingUser_ReturnsException()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var userToUpdate = new User { UserId = 1, EmailId = "test@example.com", Password = "password" };

            using (var context = new ApplicationDbContext(options))
            {
                var userDAL = new UserDAL(context);

                // Act and Assert
                var exception = await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => userDAL.UpdateUser(userToUpdate));

                // Assert
                Assert.Contains("does not exist in the store", exception.Message);
            }
        }

        [Fact]
        public void GetAllUsers_ReturnsAllUsers()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var users = new List<User>
            {
                new User { UserId = 1, EmailId = "test1@example.com", Password = "password1" },
                new User { UserId = 2, EmailId = "test2@example.com", Password = "password2" }
            };

            using (var context = new ApplicationDbContext(options))
            {
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var userDAL = new UserDAL(context);

                // Act
                var allUsers = userDAL.GetAllUsers();

                // Assert
                Assert.Equal(users.Count, allUsers.Count());
                foreach (var expectedUser in users)
                {
                    Assert.Contains(allUsers, u => u.UserId == expectedUser.UserId);
                }
            }
        }

        [Fact]
        public void GetAllUsers_EmptyList_ReturnsEmptyCollection()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var userDAL = new UserDAL(context);

                // Act
                var allUsers = userDAL.GetAllUsers();

                // Assert
                Assert.Empty(allUsers);
            }
        }

        [Fact]
        public void GetUserById_ExistingUser_ReturnsUser()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var expectedUser = new User { UserId = 1, EmailId = "test@example.com", Password = "password" };

            using (var context = new ApplicationDbContext(options))
            {
                context.Users.Add(expectedUser);
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var userDAL = new UserDAL(context);

                // Act
                var user = userDAL.GetUserById(1);

                // Assert
                Assert.NotNull(user);
                Assert.Equal(expectedUser.UserId, user.UserId);
                Assert.Equal(expectedUser.EmailId, user.EmailId);
                Assert.Equal(expectedUser.Password, user.Password);
            }
        }

        [Fact]
        public void GetUserById_NonExistingUser_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var userDAL = new UserDAL(context);

                // Act
                var user = userDAL.GetUserById(999); // Non-existing ID

                // Assert
                Assert.Null(user);
            }
        }
    }
}
