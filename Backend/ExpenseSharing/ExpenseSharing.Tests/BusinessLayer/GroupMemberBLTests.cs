using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLayer;
using BusinessLayer.Interface;
using BusinessObjectLayer;
using DataAccessLayer.Interface;
using Moq;
using Xunit;

namespace ExpenseSharing.Tests.BusinessLayer
{
    public class GroupMemberBLTests
    {
        [Fact]
        public async Task AddMemberToGroupAsync_ValidMember_AddsSuccessfully()
        {
            // Arrange
            var mockGroupMemberDAL = new Mock<IGroupMemberDAL>();
            mockGroupMemberDAL.Setup(m => m.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new User { UserId = 1 }); 

            mockGroupMemberDAL.Setup(m => m.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Group { GroupId = 1 }); 

            mockGroupMemberDAL.Setup(m => m.GetGroupMemberByUserIdAndGroupIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync((GroupMember)null); 

            mockGroupMemberDAL.Setup(m => m.AddMemberToGroupAsync(It.IsAny<GroupMember>()))
                .ReturnsAsync(new GroupMember { GroupMemberId = 1, UserId = 1, GroupId = 1 }); 

            var groupMemberBL = new GroupMemberBL(mockGroupMemberDAL.Object);
            var groupMemberToAdd = new GroupMember { UserId = 1, GroupId = 1 };

            // Act
            var addedMember = await groupMemberBL.AddMemberToGroupAsync(groupMemberToAdd);

            // Assert
            Assert.NotNull(addedMember);
            Assert.Equal(groupMemberToAdd.UserId, addedMember.UserId);
            Assert.Equal(groupMemberToAdd.GroupId, addedMember.GroupId);
        }

        [Fact]
        public async Task AddMemberToGroupAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            var mockGroupMemberDAL = new Mock<IGroupMemberDAL>();
            mockGroupMemberDAL.Setup(m => m.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((User)null); 

            var groupMemberBL = new GroupMemberBL(mockGroupMemberDAL.Object);
            var groupMemberToAdd = new GroupMember { UserId = 1, GroupId = 1 };

            // Act and Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => groupMemberBL.AddMemberToGroupAsync(groupMemberToAdd));
        }

        [Fact]
        public async Task AddMemberToGroupAsync_GroupNotFound_ThrowsException()
        {
            // Arrange
            var mockGroupMemberDAL = new Mock<IGroupMemberDAL>();
            mockGroupMemberDAL.Setup(m => m.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new User { UserId = 1 }); 

            mockGroupMemberDAL.Setup(m => m.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Group)null); 

            var groupMemberBL = new GroupMemberBL(mockGroupMemberDAL.Object);
            var groupMemberToAdd = new GroupMember { UserId = 1, GroupId = 1 };

            // Act and Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => groupMemberBL.AddMemberToGroupAsync(groupMemberToAdd));
        }

        [Fact]
        public async Task AddMemberToGroupAsync_UserAlreadyMember_ThrowsException()
        {
            // Arrange
            var mockGroupMemberDAL = new Mock<IGroupMemberDAL>();
            mockGroupMemberDAL.Setup(m => m.GetUserByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new User { UserId = 1 }); 

            mockGroupMemberDAL.Setup(m => m.GetGroupByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new Group { GroupId = 1 }); 

            mockGroupMemberDAL.Setup(m => m.GetGroupMemberByUserIdAndGroupIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new GroupMember { UserId = 1, GroupId = 1 }); 

            var groupMemberBL = new GroupMemberBL(mockGroupMemberDAL.Object);
            var groupMemberToAdd = new GroupMember { UserId = 1, GroupId = 1 };

            // Act and Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => groupMemberBL.AddMemberToGroupAsync(groupMemberToAdd));
        }

        [Fact]
        public async Task GetGroupMemberByIdAsync_ExistingId_ReturnsGroupMember()
        {
            // Arrange
            var mockGroupMemberDAL = new Mock<IGroupMemberDAL>();
            var expectedGroupMember = new GroupMember { GroupMemberId = 1 };
            mockGroupMemberDAL.Setup(m => m.GetGroupMemberByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedGroupMember);
            var groupMemberBL = new GroupMemberBL(mockGroupMemberDAL.Object);

            // Act
            var result = await groupMemberBL.GetGroupMemberByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedGroupMember.GroupMemberId, result.GroupMemberId);
        }

        [Fact]
        public async Task GetGroupMemberByIdAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var mockGroupMemberDAL = new Mock<IGroupMemberDAL>();
            mockGroupMemberDAL.Setup(m => m.GetGroupMemberByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((GroupMember)null);
            var groupMemberBL = new GroupMemberBL(mockGroupMemberDAL.Object);

            // Act
            var result = await groupMemberBL.GetGroupMemberByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetGroupMembersByGroupIdAsync_ExistingGroupId_ReturnsMembers()
        {
            // Arrange
            var mockGroupMemberDAL = new Mock<IGroupMemberDAL>();
            var expectedMembers = new List<GroupMember>
            {
                new GroupMember { GroupMemberId = 1, GroupId = 1 },
                new GroupMember { GroupMemberId = 2, GroupId = 1 }
            };
            mockGroupMemberDAL.Setup(m => m.GetGroupMembersByGroupIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedMembers);
            var groupMemberBL = new GroupMemberBL(mockGroupMemberDAL.Object);

            // Act
            var result = await groupMemberBL.GetGroupMembersByGroupIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMembers.Count, result.Count);
            Assert.Equal(expectedMembers[0].GroupId, result[0].GroupId);
            Assert.Equal(expectedMembers[1].GroupId, result[1].GroupId);
        }

        [Fact]
        public async Task GetGroupMembersByGroupIdAsync_NonExistingGroupId_ReturnsEmptyList()
        {
            // Arrange
            var mockGroupMemberDAL = new Mock<IGroupMemberDAL>();
            mockGroupMemberDAL.Setup(m => m.GetGroupMembersByGroupIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<GroupMember>());
            var groupMemberBL = new GroupMemberBL(mockGroupMemberDAL.Object);

            // Act
            var result = await groupMemberBL.GetGroupMembersByGroupIdAsync(999);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetGroupMembersByUserIdAsync_ExistingUserId_ReturnsMembers()
        {
            // Arrange
            var mockGroupMemberDAL = new Mock<IGroupMemberDAL>();
            var expectedMembers = new List<GroupMember>
            {
                new GroupMember { GroupMemberId = 1, UserId = 1 },
                new GroupMember { GroupMemberId = 2, UserId = 1 }
            };
            mockGroupMemberDAL.Setup(m => m.GetGroupMembersByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(expectedMembers);
            var groupMemberBL = new GroupMemberBL(mockGroupMemberDAL.Object);

            // Act
            var result = await groupMemberBL.GetGroupMembersByUserIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedMembers.Count, result.Count);
            Assert.Equal(expectedMembers[0].UserId, result[0].UserId);
            Assert.Equal(expectedMembers[1].UserId, result[1].UserId);
        }

        [Fact]
        public async Task GetGroupMembersByUserIdAsync_NonExistingUserId_ReturnsEmptyList()
        {
            // Arrange
            var mockGroupMemberDAL = new Mock<IGroupMemberDAL>();
            mockGroupMemberDAL.Setup(m => m.GetGroupMembersByUserIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new List<GroupMember>());
            var groupMemberBL = new GroupMemberBL(mockGroupMemberDAL.Object);

            // Act
            var result = await groupMemberBL.GetGroupMembersByUserIdAsync(999);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
