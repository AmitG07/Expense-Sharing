using BusinessObjectLayer;
using DataAccessLayer;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseSharing.Tests.DataAccessLayer
{
    public class GroupMemberDALTests
    {
        private ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddMemberToGroupAsync_ValidMember_AddsMemberToGroup()
        {
            // Arrange
            using (var context = CreateDbContext())
            {
                var groupMember = new GroupMember { /* Initialize group member properties */ };
                var groupDAL = new GroupMemberDAL(context);

                // Act
                var addedMember = await groupDAL.AddMemberToGroupAsync(groupMember);

                // Assert
                Assert.NotNull(addedMember);
            }
        }

        [Fact]
        public async Task GetUserByIdAsync_ExistingUserId_ReturnsUser()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;

            var userId = 1;
            var user = new User { UserId = userId, Name = "TestUser" };

            using (var context = new ApplicationDbContext(options))
            {
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            // Mocking DbContext and DAL
            using (var mockContext = new ApplicationDbContext(options))
            {
                var groupDAL = new GroupMemberDAL(mockContext);

                // Act
                var retrievedUser = await groupDAL.GetUserByIdAsync(userId);

                // Assert
                Assert.NotNull(retrievedUser);
                Assert.Equal(user.UserId, retrievedUser.UserId);
            }
        }

        [Fact]
        public async Task GetUserByIdAsync_NonExistingUserId_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;

            // Mocking DbContext and DAL
            using (var mockContext = new ApplicationDbContext(options))
            {
                var groupDAL = new GroupMemberDAL(mockContext);

                // Act
                var retrievedUser = await groupDAL.GetUserByIdAsync(999);

                // Assert
                Assert.Null(retrievedUser);
            }
        }

        [Fact]
        public async Task GetGroupByIdAsync_ExistingGroupId_ReturnsGroup()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;

            var groupId = 1;
            var group = new Group { GroupId = groupId, GroupName = "TestGroup" };

            using (var context = new ApplicationDbContext(options))
            {
                context.Groups.Add(group);
                await context.SaveChangesAsync();
            }

            // Mocking DbContext and DAL
            using (var mockContext = new ApplicationDbContext(options))
            {
                var groupDAL = new GroupMemberDAL(mockContext);

                // Act
                var retrievedGroup = await groupDAL.GetGroupByIdAsync(groupId);

                // Assert
                Assert.NotNull(retrievedGroup);
                Assert.Equal(group.GroupId, retrievedGroup.GroupId);
            }
        }

        [Fact]
        public async Task GetGroupByIdAsync_NonExistingGroupId_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;

            // Mocking DbContext and DAL
            using (var mockContext = new ApplicationDbContext(options))
            {
                var groupDAL = new GroupMemberDAL(mockContext);

                // Act
                var retrievedGroup = await groupDAL.GetGroupByIdAsync(999);

                // Assert
                Assert.Null(retrievedGroup);
            }
        }

        [Fact]
        public async Task UpdateGroupMemberAsync_ValidMember_ReturnsUpdatedMember()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;

            var groupMemberId = 1;
            var initialGroupMember = new GroupMember
            {
                GroupMemberId = groupMemberId,
                UserId = 1,
                GroupId = 1,
                GivenAmount = 0,
                TakenAmount = 0
            };

            using (var context = new ApplicationDbContext(options))
            {
                context.GroupMembers.Add(initialGroupMember);
                await context.SaveChangesAsync();
            }

            var updatedGroupMember = new GroupMember
            {
                GroupMemberId = groupMemberId,
                UserId = 1,
                GroupId = 1,
                GivenAmount = 50,
                TakenAmount = 10
            };

            // Mocking DbContext and DAL
            using (var mockContext = new ApplicationDbContext(options))
            {
                var groupDAL = new GroupMemberDAL(mockContext);

                // Act
                var result = await groupDAL.UpdateGroupMemberAsync(updatedGroupMember);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(updatedGroupMember.GroupMemberId, result.GroupMemberId);
                Assert.Equal(updatedGroupMember.GivenAmount, result.GivenAmount);
                Assert.Equal(updatedGroupMember.TakenAmount, result.TakenAmount);
            }
        }

        [Fact]
        public async Task GetGroupMemberByIdAsync_ExistingGroupMemberId_ReturnsGroupMember()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database_GetGroupMemberByIdAsync")
                .Options;

            var groupMemberId = 1;
            var groupMember = new GroupMember
            {
                GroupMemberId = groupMemberId,
                UserId = 1,
                GroupId = 1,
                GivenAmount = 0,
                TakenAmount = 0
            };

            // Seed data
            using (var context = new ApplicationDbContext(options))
            {
                context.GroupMembers.Add(groupMember);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var groupDAL = new GroupMemberDAL(context);
                var retrievedGroupMember = await groupDAL.GetGroupMemberByIdAsync(groupMemberId);

                // Assert
                Assert.NotNull(retrievedGroupMember);
                Assert.Equal(groupMemberId, retrievedGroupMember.GroupMemberId);
                Assert.Equal(groupMember.UserId, retrievedGroupMember.UserId);
                Assert.Equal(groupMember.GroupId, retrievedGroupMember.GroupId);
                Assert.Equal(groupMember.GivenAmount, retrievedGroupMember.GivenAmount);
                Assert.Equal(groupMember.TakenAmount, retrievedGroupMember.TakenAmount);
            }
        }

        [Fact]
        public async Task GetGroupMemberByIdAsync_NonExistingGroupMemberId_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;

            // Mocking DbContext and DAL
            using (var mockContext = new ApplicationDbContext(options))
            {
                var groupDAL = new GroupMemberDAL(mockContext);

                // Act
                var retrievedGroupMember = await groupDAL.GetGroupMemberByIdAsync(999);

                // Assert
                Assert.Null(retrievedGroupMember);
            }
        }

        /*[Fact]
        public async Task GetGroupMembersByGroupIdAsync_ExistingGroupId_ReturnsGroupMembers()
        {
            // Arrange
            var groupId = 1;
            var groupMembers = new List<GroupMember>
            {
                new GroupMember { GroupMemberId = 1, GroupId = groupId, UserId = 1, UserExpense = 0, GivenAmount = 0, TakenAmount = 0 },
                new GroupMember { GroupMemberId = 2, GroupId = groupId, UserId = 2, UserExpense = 0, GivenAmount = 0, TakenAmount = 0 }
            };

            // Insert groupMembers into the database
            using (var context = CreateDbContext())
            {
                context.GroupMembers.AddRange(groupMembers);
                await context.SaveChangesAsync();
            }

            List<GroupMember> retrievedGroupMembers;

            try
            {
                // Act
                using (var context = CreateDbContext())
                {
                    var groupDAL = new GroupMemberDAL(context);
                    retrievedGroupMembers = await groupDAL.GetGroupMembersByGroupIdAsync(groupId);
                }

                // Assert
                Assert.NotNull(retrievedGroupMembers);
                Assert.Equal(groupMembers.Count, retrievedGroupMembers.Count);
                Assert.True(retrievedGroupMembers.All(gm => gm.GroupId == groupId));
            }
            catch (Exception ex)
            {
                // Log any exceptions for debugging purposes
                Console.WriteLine($"Exception occurred: {ex.Message}");
                throw;
            }
        }*/

        [Fact]
        public async Task GetGroupMembersByGroupIdAsync_NonExistingGroupId_ReturnsEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;

            // Mocking DbContext and DAL
            using (var mockContext = new ApplicationDbContext(options))
            {
                var groupDAL = new GroupMemberDAL(mockContext);

                // Act
                var retrievedGroupMembers = await groupDAL.GetGroupMembersByGroupIdAsync(999);

                // Assert
                Assert.NotNull(retrievedGroupMembers);
                Assert.Empty(retrievedGroupMembers);
            }
        }

        /*[Fact]
        public async Task GetGroupMembersByUserIdAsync_ExistingUserId_ReturnsGroupMembers()
        {
            // Arrange
            var userId = 1;
            var groupMembers = new List<GroupMember>
            {
                new GroupMember { GroupMemberId = 1, GroupId = 1, UserId = userId, GivenAmount = 0, TakenAmount = 0 },
                new GroupMember { GroupMemberId = 2, GroupId = 2, UserId = userId, GivenAmount = 0, TakenAmount = 0 }
            };

            // Create options for in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            // Seed the in-memory database with data
            using (var context = new ApplicationDbContext(options))
            {
                context.GroupMembers.AddRange(groupMembers);
                await context.SaveChangesAsync();
            }

            // Act
            List<GroupMember> retrievedGroupMembers;
            using (var context = new ApplicationDbContext(options))
            {
                var groupDAL = new GroupMemberDAL(context);
                retrievedGroupMembers = await groupDAL.GetGroupMembersByUserIdAsync(userId);
            }

            // Assert
            Assert.NotNull(retrievedGroupMembers);
            Assert.Equal(groupMembers.Count, retrievedGroupMembers.Count); 
            Assert.All(retrievedGroupMembers, gm => Assert.Equal(userId, gm.UserId));
        }*/

        [Fact]
        public async Task GetGroupMembersByUserIdAsync_NonExistingUserId_ReturnsEmptyList()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;

            // Mocking DbContext and DAL
            using (var mockContext = new ApplicationDbContext(options))
            {
                var groupDAL = new GroupMemberDAL(mockContext);

                // Act
                var retrievedGroupMembers = await groupDAL.GetGroupMembersByUserIdAsync(999);

                // Assert
                Assert.NotNull(retrievedGroupMembers);
                Assert.Empty(retrievedGroupMembers);
            }
        }

        [Fact]
        public async Task GetGroupMemberByUserIdAndGroupIdAsync_ExistingUserIdAndGroupId_ReturnsGroupMember()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database_GetGroupMemberByUserIdAndGroupIdAsync")
                .Options;

            var userId = 1;
            var groupId = 1;
            var groupMember = new GroupMember
            {
                GroupMemberId = 1,
                UserId = userId,
                GroupId = groupId,
                GivenAmount = 0,
                TakenAmount = 0
            };

            // Seed data
            using (var context = new ApplicationDbContext(options))
            {
                context.GroupMembers.Add(groupMember);
                await context.SaveChangesAsync();
            }

            // Act
            using (var context = new ApplicationDbContext(options))
            {
                var groupDAL = new GroupMemberDAL(context);
                var retrievedGroupMember = await groupDAL.GetGroupMemberByUserIdAndGroupIdAsync(userId, groupId);

                // Assert
                Assert.NotNull(retrievedGroupMember);
                Assert.Equal(userId, retrievedGroupMember.UserId);
                Assert.Equal(groupId, retrievedGroupMember.GroupId);
            }
        }

        [Fact]
        public async Task GetGroupMemberByUserIdAndGroupIdAsync_NonExistingUserIdAndGroupId_ReturnsNull()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "test_database")
                .Options;

            // Mocking DbContext and DAL
            using (var mockContext = new ApplicationDbContext(options))
            {
                var groupDAL = new GroupMemberDAL(mockContext);

                // Act
                var retrievedGroupMember = await groupDAL.GetGroupMemberByUserIdAndGroupIdAsync(999, 999);

                // Assert
                Assert.Null(retrievedGroupMember);
            }
        }
    }
}
