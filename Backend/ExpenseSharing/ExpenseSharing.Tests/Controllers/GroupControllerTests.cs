using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using ExpenseSharing.Controllers;
using BusinessObjectLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ExpenseSharing.Tests.Controllers
{
    public class GroupControllerTests
    {
        private readonly Mock<ILogger<GroupController>> _loggerMock = new Mock<ILogger<GroupController>>();
        private readonly Mock<IGroupBL> _groupBlMock = new Mock<IGroupBL>();
        private readonly Mock<IGroupMemberBL> _groupMemberBlMock = new Mock<IGroupMemberBL>();
        private readonly Mock<IUserBL> _userBlMock = new Mock<IUserBL>();
        private readonly Mock<IExpenseBL> _expenseBlMock = new Mock<IExpenseBL>();

        [Fact]
        public async Task CreateGroup_ValidModel_ReturnsCreatedAtAction()
        {
            // Arrange
            var groupModel = new Group { GroupId = 1, GroupName = "Test Group" };

            // Setup CreateGroupAsync to return the groupModel directly
            _groupBlMock.Setup(x => x.CreateGroupAsync(It.IsAny<Group>()))
                        .Returns((Group group) => Task.FromResult(group));

            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object,
                                                _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var result = await controller.CreateGroup(groupModel) as CreatedAtActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(nameof(GroupController.GetGroupByGroupId), result.ActionName);
            Assert.Equal(groupModel.GroupId, result.RouteValues["id"]);
            Assert.Equal(groupModel, result.Value);
        }

        [Fact]
        public async Task CreateGroup_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);
            controller.ModelState.AddModelError("GroupName", "Group name is required");

            // Act
            var result = await controller.CreateGroup(null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetAllGroups_ReturnsListOfGroups()
        {
            // Arrange
            var groups = new List<Group> { new Group { GroupId = 1, GroupName = "Group 1" }, new Group { GroupId = 2, GroupName = "Group 2" } };
            _groupBlMock.Setup(x => x.GetAllGroups()).ReturnsAsync(groups);
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var result = await controller.GetAllGroups();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedGroups = Assert.IsAssignableFrom<IEnumerable<Group>>(okResult.Value);
            Assert.Equal(groups.Count, returnedGroups.Count());
        }

        [Fact]
        public async Task GetGroupByGroupId_ExistingGroupId_ReturnsGroup()
        {
            // Arrange
            var groupId = 1;
            var group = new Group { GroupId = groupId, GroupName = "Test Group" };
            _groupBlMock.Setup(x => x.GetGroupByGroupIdAsync(groupId)).ReturnsAsync(group);
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var result = await controller.GetGroupByGroupId(groupId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(group, result.Value);
        }

        [Fact]
        public async Task GetGroupsByUserId_ExistingUserId_ReturnsGroups()
        {
            // Arrange
            var userId = 1;
            var groups = new List<Group> { new Group { GroupId = 1, GroupName = "Group 1" }, new Group { GroupId = 2, GroupName = "Group 2" } };
            _groupBlMock.Setup(x => x.GetGroupsByUserIdAsync(userId)).ReturnsAsync(groups);
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var actionResult = await controller.GetGroupsByUserId(userId);

            // Assert
            Assert.NotNull(actionResult);

            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(200, okResult.StatusCode);

            var returnedGroups = Assert.IsAssignableFrom<IEnumerable<Group>>(okResult.Value);
            Assert.Equal(groups.Count, returnedGroups.Count());
        }

        [Fact]
        public async Task UpdateGroup_ValidModel_ReturnsOk()
        {
            // Arrange
            var groupId = 1;
            var groupModel = new Group { GroupId = groupId, GroupName = "Updated Group" };
            _groupBlMock.Setup(x => x.UpdateGroupAsync(groupModel)).Returns(Task.CompletedTask);
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var result = await controller.UpdateGroup(groupId, groupModel) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Group updated successfully.", result.Value.GetType().GetProperty("message")?.GetValue(result.Value));
        }

        [Fact]
        public async Task DeleteGroup_ExistingGroupId_ReturnsOk()
        {
            // Arrange
            var groupId = 1;
            var group = new Group { GroupId = groupId, GroupName = "Test Group" };
            var groupMembers = new List<GroupMember> { new GroupMember { UserId = 1, GroupId = groupId, UserExpense = 0.0 } };
            _groupBlMock.Setup(x => x.GetGroupByGroupIdAsync(groupId)).ReturnsAsync(group);
            _expenseBlMock.Setup(x => x.GetExpensesByGroupId(groupId)).Returns(new List<Expense>());
            _groupMemberBlMock.Setup(x => x.GetGroupMembersByGroupIdAsync(groupId)).ReturnsAsync(groupMembers);
            _userBlMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(new User { UserId = 1, AvailableBalance = 0 });
            _userBlMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _groupBlMock.Setup(x => x.DeleteGroupAsync(groupId)).Returns(Task.CompletedTask);
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var result = await controller.DeleteGroup(groupId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Group deleted successfully.", result.Value.GetType().GetProperty("message")?.GetValue(result.Value));
        }

        [Fact]
        public async Task DeleteGroup_GroupWithExpenses_ReturnsOk()
        {
            // Arrange
            var groupId = 1;
            var group = new Group { GroupId = groupId, GroupName = "Test Group" };
            var expenses = new List<Expense> { new Expense { ExpenseId = 1, GroupId = groupId } };
            var groupMembers = new List<GroupMember> { new GroupMember { UserId = 1, GroupId = groupId, UserExpense = 10.0 } };
            _groupBlMock.Setup(x => x.GetGroupByGroupIdAsync(groupId)).ReturnsAsync(group);
            _expenseBlMock.Setup(x => x.GetExpensesByGroupId(groupId)).Returns(expenses);
            _groupMemberBlMock.Setup(x => x.GetGroupMembersByGroupIdAsync(groupId)).ReturnsAsync(groupMembers);
            _userBlMock.Setup(x => x.GetUserByIdAsync(It.IsAny<int>())).ReturnsAsync(new User { UserId = 1, AvailableBalance = 0 });
            _userBlMock.Setup(x => x.UpdateUserAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _groupBlMock.Setup(x => x.DeleteGroupAsync(groupId)).Returns(Task.CompletedTask);
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var result = await controller.DeleteGroup(groupId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Group deleted successfully.", result.Value.GetType().GetProperty("message")?.GetValue(result.Value));
        }

        [Fact]
        public async Task GetGroupDetails_ExistingGroupId_ReturnsGroup()
        {
            // Arrange
            var groupId = 1;
            var group = new Group { GroupId = groupId, GroupName = "Test Group" };
            _groupBlMock.Setup(x => x.GetGroupDetailsAsync(groupId)).ReturnsAsync(group);
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var result = await controller.GetGroupDetails(groupId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(group, result.Value);
        }

        [Fact]
        public async Task GetGroupByGroupId_NonExistingGroupId_ReturnsNotFound()
        {
            // Arrange
            var groupId = 999; // Non-existing group id
            _groupBlMock.Setup(x => x.GetGroupByGroupIdAsync(groupId)).ReturnsAsync((Group)null);
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var result = await controller.GetGroupByGroupId(groupId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateGroup_NullModel_ReturnsBadRequest()
        {
            // Arrange
            var groupId = 1;
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var result = await controller.UpdateGroup(groupId, null);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task DeleteGroup_NonExistingGroupId_ReturnsNotFound()
        {
            // Arrange
            var groupId = 999; // Non-existing group id
            _groupBlMock.Setup(x => x.GetGroupByGroupIdAsync(groupId)).ReturnsAsync((Group)null);
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var result = await controller.DeleteGroup(groupId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetGroupDetails_NonExistingGroupId_ReturnsNotFound()
        {
            // Arrange
            var groupId = 999; // Non-existing group id
            _groupBlMock.Setup(x => x.GetGroupDetailsAsync(groupId)).ReturnsAsync((Group)null);
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var result = await controller.GetGroupDetails(groupId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetGroupMembersByUserId_ExistingUserId_ReturnsMembers()
        {
            // Arrange
            var userId = 1;
            var members = new List<GroupMember> { new GroupMember { UserId = 1, GroupId = 1, UserExpense = 0.0 } };
            _groupMemberBlMock.Setup(x => x.GetGroupMembersByUserIdAsync(userId)).ReturnsAsync(members);
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var result = await controller.GetGroupMembersByUserId(userId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returnedMembers = Assert.IsAssignableFrom<IEnumerable<GroupMember>>(result.Value);
            Assert.Equal(members.Count, returnedMembers.Count());
        }

        [Fact]
        public async Task GetGroupMembersByUserId_NonExistingUserId_ReturnsEmptyList()
        {
            // Arrange
            var userId = 999; // Non-existing user id
            var emptyList = new List<GroupMember>();
            _groupMemberBlMock.Setup(x => x.GetGroupMembersByUserIdAsync(userId)).ReturnsAsync(emptyList);
            var controller = new GroupController(_loggerMock.Object, _groupBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object, _expenseBlMock.Object);

            // Act
            var actionResult = await controller.GetGroupMembersByUserId(userId);

            // Assert
            Assert.NotNull(actionResult);
        }
    }
}
