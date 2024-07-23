using BusinessLayer.Interface;
using BusinessObjectLayer;
using ExpenseSharing.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseSharing.Tests.Controllers
{
    public class GroupMemberControllerTests
    {
        private readonly Mock<IGroupMemberBL> _groupMemberBlMock;
        private readonly GroupMemberController _controller;

        public GroupMemberControllerTests()
        {
            _groupMemberBlMock = new Mock<IGroupMemberBL>();
            _controller = new GroupMemberController(_groupMemberBlMock.Object);
        }

        [Fact]
        public async Task AddMemberToGroup_ValidGroupMember_ReturnsOkResult()
        {
            // Arrange
            var groupMember = new GroupMember { GroupId = 1, UserId = 1 };

            _groupMemberBlMock.Setup(x => x.AddMemberToGroupAsync(groupMember))
                .ReturnsAsync(groupMember); // Mocking the return value

            // Act
            var result = await _controller.AddMemberToGroup(groupMember);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<GroupMember>(okResult.Value);
            Assert.Equal(groupMember.GroupId, model.GroupId);
            Assert.Equal(groupMember.UserId, model.UserId);
        }

        /*[Fact]
        public async Task AddMemberToGroup_InvalidGroupMember_ReturnsBadRequest()
        {
            // Arrange
            var invalidGroupMember = new GroupMember { GroupId = 99, UserId = 99 };

            _groupMemberBlMock
                .Setup(x => x.AddMemberToGroupAsync(It.IsAny<GroupMember>()))
                .ThrowsAsync(new InvalidOperationException("Invalid group member"));

            // Act
            var result = await _controller.AddMemberToGroup(invalidGroupMember);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = badRequestResult.Value as SerializableError;

            Assert.NotNull(errorResponse); // Ensure errorResponse is not null

            // Check if there is an error message with key "message"
            Assert.True(errorResponse.ContainsKey("message"));

            // Validate the content of the "message" array
            var messages = (string[])errorResponse["message"];
            Assert.Contains("Invalid group member", messages);
        }*/

        [Fact]
        public async Task GetGroupMemberById_ExistingId_ReturnsOkResult()
        {
            // Arrange
            var groupId = 1;
            var groupMember = new GroupMember { GroupMemberId = 1, GroupId = groupId, UserId = 1 };

            _groupMemberBlMock.Setup(x => x.GetGroupMemberByIdAsync(groupId))
                .ReturnsAsync(groupMember);

            // Act
            var result = await _controller.GetGroupMemberById(groupId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<GroupMember>(okResult.Value);
            Assert.Equal(groupId, model.GroupId);
            Assert.Equal(1, model.GroupMemberId);
        }

        [Fact]
        public async Task GetGroupMemberById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var groupId = 999; // Non-existing GroupMemberId

            _groupMemberBlMock.Setup(x => x.GetGroupMemberByIdAsync(groupId))
                .ReturnsAsync((GroupMember)null); // Mocking null return

            // Act
            var result = await _controller.GetGroupMemberById(groupId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetGroupMembersByGroupId_ExistingGroupId_ReturnsOkResult()
        {
            // Arrange
            var groupId = 1;
            var groupMembers = new List<GroupMember>
            {
                new GroupMember { GroupId = groupId, UserId = 1 },
                new GroupMember { GroupId = groupId, UserId = 2 }
            };

            _groupMemberBlMock.Setup(x => x.GetGroupMembersByGroupIdAsync(groupId))
                .ReturnsAsync(groupMembers); // Mocking the return value

            // Act
            var result = await _controller.GetGroupMembersByGroupId(groupId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var models = Assert.IsType<List<GroupMember>>(okResult.Value);
            Assert.Equal(2, models.Count);
        }

        [Fact]
        public async Task GetGroupMembersByGroupId_NonExistingGroupId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingGroupId = 999; // Non-existing groupId

            _groupMemberBlMock.Setup(x => x.GetGroupMembersByGroupIdAsync(nonExistingGroupId))
                .ReturnsAsync(new List<GroupMember>()); // Mocking an empty list for non-existing group

            // Act
            var result = await _controller.GetGroupMembersByGroupId(nonExistingGroupId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetGroupMembersByUserId_ExistingUserId_ReturnsOkResult()
        {
            // Arrange
            var userId = 1;
            var groupMembers = new List<GroupMember>
            {
                new GroupMember { GroupId = 1, UserId = userId },
                new GroupMember { GroupId = 2, UserId = userId }
            };

            _groupMemberBlMock.Setup(x => x.GetGroupMembersByUserIdAsync(userId))
                .ReturnsAsync(groupMembers); // Mocking the return value

            // Act
            var result = await _controller.GetGroupMembersByUserId(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var models = Assert.IsType<List<GroupMember>>(okResult.Value);
            Assert.Equal(2, models.Count);
        }

        [Fact]
        public async Task GetGroupMembersByUserId_NonExistingUserId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingUserId = 999; // Non-existing userId

            _groupMemberBlMock.Setup(x => x.GetGroupMembersByUserIdAsync(nonExistingUserId))
                .ReturnsAsync(new List<GroupMember>()); // Mocking an empty list for non-existing user

            // Act
            var result = await _controller.GetGroupMembersByUserId(nonExistingUserId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
