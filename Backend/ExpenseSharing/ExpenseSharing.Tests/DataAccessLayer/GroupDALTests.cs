using BusinessObjectLayer;
using DataAccessLayer;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseSharing.Tests.DataAccessLayer
{
    public class GroupDALTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IGroupDAL _groupDAL;
        private readonly IConfiguration _configuration;

        public GroupDALTests()
        {
            // Initialize DbContextOptions for in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Create instance of ApplicationDbContext using in-memory database
            _context = new ApplicationDbContext(options);

            // Seed the database with test data
            SeedDatabase();

            // Mock IConfiguration
            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "ConnectionStrings:DefaultConnection", "Data Source=IN-PG0352EK;Initial Catalog = ExpenseSharingApp;Integrated Security =True" }
                })
                .Build();

            // Initialize GroupDAL with the DbContext and mocked IConfiguration
            _groupDAL = new GroupDAL(_context, _configuration);
        }

        private void SeedDatabase()
        {
            var groups = new List<Group>
            {
                new Group
                {
                    GroupId = 1,
                    GroupName = "Group 1",
                    GroupAdminId = 1,
                    TotalMembers = 2,
                    TotalExpense = 100.0
                },
                new Group
                {
                    GroupId = 2,
                    GroupName = "Group 2",
                    GroupAdminId = 2,
                    TotalMembers = 1,
                    TotalExpense = 50.0 
                }
            };

            var users = new List<User>
            {
                new User { UserId = 1, Name = "User 1", EmailId = "user1@example.com" },
                new User { UserId = 2, Name = "User 2", EmailId = "user2@example.com" }
            };

            var groupMembers = new List<GroupMember>
            {
                new GroupMember { GroupMemberId = 1, GroupId = 1, UserId = 1 },
                new GroupMember { GroupMemberId = 2, GroupId = 1, UserId = 2 },
                new GroupMember { GroupMemberId = 3, GroupId = 2, UserId = 2 }
            };

            var expenses = new List<Expense>
            {
                new Expense { ExpenseId = 1, GroupId = 1, PaidByUserId = 1, ExpenseAmount = 100.0 },
                new Expense { ExpenseId = 2, GroupId = 2, PaidByUserId = 2, ExpenseAmount = 50.0 }
            };

            _context.AddRange(groups);
            _context.AddRange(users);
            _context.AddRange(groupMembers);
            _context.AddRange(expenses);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            // Dispose the DbContext after each test
            _context.Dispose();
        }

        [Fact]
        public async Task CreateGroupAsync_ValidGroup_ReturnsCreatedGroup()
        {
            // Arrange
            var group = new Group { GroupId = 3, GroupName = "New Group", GroupAdminId = 3 };

            // Act
            await _groupDAL.CreateGroupAsync(group);
            var addedGroup = await _context.Groups.FindAsync(group.GroupId);

            // Assert
            Assert.NotNull(addedGroup);
            Assert.Equal(group.GroupName, addedGroup.GroupName);
        }

        [Fact]
        public async Task GetGroupByGroupIdAsync_ExistingGroupId_ReturnsGroup()
        {
            // Arrange
            var groupId = 1;

            // Act
            var group = await _groupDAL.GetGroupByGroupIdAsync(groupId);

            // Assert
            Assert.NotNull(group);
            Assert.Equal(groupId, group.GroupId);
        }

        [Fact]
        public async Task GetGroupByGroupIdAsync_NonExistingGroupId_ReturnsNull()
        {
            // Arrange
            var groupId = 999; // Non-existing group Id

            // Act
            var group = await _groupDAL.GetGroupByGroupIdAsync(groupId);

            // Assert
            Assert.Null(group);
        }

        [Fact]
        public async Task GetGroupsByUserIdAsync_ExistingUserId_ReturnsGroups()
        {
            // Arrange
            var userId = 1;

            // Act
            var groups = await _groupDAL.GetGroupsByUserIdAsync(userId);

            // Assert
            Assert.NotNull(groups);
            Assert.True(groups.Any()); 
        }

        [Fact]
        public async Task GetGroupsByUserIdAsync_NonExistingUserId_ReturnsEmptyList()
        {
            // Arrange
            var userId = 999; // Non-existing user Id

            // Act
            var groups = await _groupDAL.GetGroupsByUserIdAsync(userId);

            // Assert
            Assert.NotNull(groups);
            Assert.Empty(groups);
        }

        [Fact]
        public async Task UpdateGroupAsync_ValidGroup_UpdatesGroup()
        {
            // Arrange
            var groupId = 1;
            var updatedGroupName = "Updated Group Name";

            // Retrieve the existing group
            var existingGroup = await _context.Groups.FindAsync(groupId);
            Assert.NotNull(existingGroup);

            // Modify the group
            existingGroup.GroupName = updatedGroupName;

            // Detach any existing entity with the same key from the context
            _context.Entry(existingGroup).State = EntityState.Detached;

            // Act
            await _groupDAL.UpdateGroupAsync(existingGroup);

            // Retrieve the updated group
            var updatedGroup = await _context.Groups.FindAsync(groupId);

            // Assert
            Assert.NotNull(updatedGroup);
            Assert.Equal(updatedGroupName, updatedGroup.GroupName);
        }

        [Fact]
        public async Task UpdateGroupAsync_InvalidGroup_ThrowsDbUpdateConcurrencyException()
        {
            // Arrange
            var group = new Group { GroupId = 999, GroupName = "Invalid Group" }; // Non-existing group Id

            // Act and Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => _groupDAL.UpdateGroupAsync(group));
        }

        [Fact]
        public async Task DeleteGroupAsync_ExistingGroupId_DeletesGroup()
        {
            // Arrange
            var groupId = 2;

            // Act
            await _groupDAL.DeleteGroupAsync(groupId);
            var deletedGroup = await _context.Groups.FindAsync(groupId);

            // Assert
            Assert.Null(deletedGroup);
        }

        [Fact]
        public async Task DeleteGroupAsync_NonExistingGroupId_DoesNotThrow()
        {
            // Arrange
            var groupId = 999; // Non-existing group Id

            // Act
            await _groupDAL.DeleteGroupAsync(groupId);

            // Assert
            Assert.True(true); // Placeholder assertion
        }

        [Fact]
        public async Task GetAllGroups_ReturnsAllGroups()
        {
            // Act
            var groups = await _groupDAL.GetAllGroups();

            // Assert
            Assert.NotNull(groups);
            Assert.Equal(2, groups.Count());
        }

        /*[Fact]
        public async Task GetGroupDetailsAsync_ExistingGroupId_ReturnsGroupWithDetails()
        {
            // Arrange
            var groupId = 1;

            try
            {
                // Act
                var group = await _groupDAL.GetGroupDetailsAsync(groupId);

                // Assert
                Assert.NotNull(group); 
                Assert.Equal(groupId, group.GroupId);

                Assert.NotEmpty(group.GroupMember);
                Assert.NotEmpty(group.Expense);
                Assert.True(group.TotalMembers <= 10); 

                foreach (var member in group.GroupMember)
                {
                    Assert.NotNull(member.User); 
                }

                foreach (var expense in group.Expense)
                {
                    Assert.NotNull(expense.ExpenseSplit); 
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                throw; 
            }
        }*/

        [Fact]
        public async Task GetGroupDetailsAsync_NonExistingGroupId_ReturnsNull()
        {
            // Arrange
            var groupId = 999; // Non-existing group Id

            // Act
            var group = await _groupDAL.GetGroupDetailsAsync(groupId);

            // Assert
            Assert.Null(group);
        }
    }
}
