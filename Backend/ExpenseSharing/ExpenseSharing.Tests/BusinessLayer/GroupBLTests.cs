using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer;
using BusinessLayer.Interface;
using BusinessObjectLayer;
using DataAccessLayer.Interface;
using Moq;
using Xunit;

namespace ExpenseSharing.Tests.BusinessLayer
{
    public class GroupBLTests
    {
        [Fact]
        public async Task CreateGroupAsync_ValidGroup_CreatesGroupAndAddsAdminAsMember()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();

            var groupBL = new GroupBL(groupDalMock.Object, groupMemberDalMock.Object);

            var group = new Group
            {
                GroupId = 1,
                GroupName = "Test Group",
                GroupAdminId = 1
            };

            // Setup mock behavior for DAL methods
            groupDalMock.Setup(m => m.CreateGroupAsync(It.IsAny<Group>())).Returns(Task.CompletedTask);

            // Corrected setup for GroupMemberDAL mock
            groupMemberDalMock.Setup(m => m.AddMemberToGroupAsync(It.IsAny<GroupMember>()))
                              .Returns(Task.FromResult<GroupMember>(null));

            // Act
            await groupBL.CreateGroupAsync(group);

            // Assert
            groupDalMock.Verify(m => m.CreateGroupAsync(It.IsAny<Group>()), Times.Once);
            groupMemberDalMock.Verify(m => m.AddMemberToGroupAsync(It.IsAny<GroupMember>()), Times.Once);
        }

        [Fact]
        public async Task CreateGroupAsync_NullGroup_ThrowsArgumentNullException()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();

            var groupBL = new GroupBL(groupDalMock.Object, groupMemberDalMock.Object);

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => groupBL.CreateGroupAsync(null));
        }

        [Fact]
        public async Task GetGroupByGroupIdAsync_ExistingGroupId_ReturnsGroup()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();

            var groupBL = new GroupBL(groupDalMock.Object, groupMemberDalMock.Object);

            var expectedGroup = new Group
            {
                GroupId = 1,
                GroupName = "Test Group",
                GroupAdminId = 1
            };

            groupDalMock.Setup(m => m.GetGroupByGroupIdAsync(1)).ReturnsAsync(expectedGroup);

            // Act
            var result = await groupBL.GetGroupByGroupIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedGroup.GroupId, result.GroupId);
            Assert.Equal(expectedGroup.GroupName, result.GroupName);
        }

        [Fact]
        public async Task GetGroupByGroupIdAsync_NonExistingGroupId_ReturnsNull()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();

            var groupBL = new GroupBL(groupDalMock.Object, groupMemberDalMock.Object);

            groupDalMock.Setup(m => m.GetGroupByGroupIdAsync(It.IsAny<int>())).ReturnsAsync((Group)null);

            // Act
            var result = await groupBL.GetGroupByGroupIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetGroupsByUserIdAsync_ExistingUserId_ReturnsGroups()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();

            var groupBL = new GroupBL(groupDalMock.Object, groupMemberDalMock.Object);

            var expectedGroups = new List<Group>
            {
                new Group { GroupId = 1, GroupName = "Group 1", GroupAdminId = 1 },
                new Group { GroupId = 2, GroupName = "Group 2", GroupAdminId = 1 },
            };

            groupDalMock.Setup(m => m.GetGroupsByUserIdAsync(1)).ReturnsAsync(expectedGroups);

            // Act
            var result = await groupBL.GetGroupsByUserIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedGroups.Count, result.Count());
            foreach (var expectedGroup in expectedGroups)
            {
                Assert.Contains(result, g => g.GroupId == expectedGroup.GroupId);
            }
        }

        [Fact]
        public async Task GetGroupsByUserIdAsync_NonExistingUserId_ReturnsEmptyCollection()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();

            var groupBL = new GroupBL(groupDalMock.Object, groupMemberDalMock.Object);

            groupDalMock.Setup(m => m.GetGroupsByUserIdAsync(It.IsAny<int>())).ReturnsAsync(new List<Group>());

            // Act
            var result = await groupBL.GetGroupsByUserIdAsync(999);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task UpdateGroupAsync_ValidGroup_UpdatesSuccessfully()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();

            var groupBL = new GroupBL(groupDalMock.Object, groupMemberDalMock.Object);

            var groupToUpdate = new Group
            {
                GroupId = 1,
                GroupName = "Updated Group Name",
                GroupAdminId = 1
            };

            groupDalMock.Setup(m => m.UpdateGroupAsync(groupToUpdate)).Returns(Task.CompletedTask);

            // Act
            await groupBL.UpdateGroupAsync(groupToUpdate);

            // Assert
            groupDalMock.Verify(m => m.UpdateGroupAsync(groupToUpdate), Times.Once);
        }

        [Fact]
        public async Task UpdateGroupAsync_NullGroup_ThrowsArgumentNullException()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();

            var groupBL = new GroupBL(groupDalMock.Object, groupMemberDalMock.Object);

            // Act and Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => groupBL.UpdateGroupAsync(null));
        }

        [Fact]
        public async Task DeleteGroupAsync_ExistingGroupId_DeletesSuccessfully()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();

            var groupBL = new GroupBL(groupDalMock.Object, groupMemberDalMock.Object);

            var groupIdToDelete = 1;

            groupDalMock.Setup(m => m.DeleteGroupAsync(groupIdToDelete)).Returns(Task.CompletedTask);

            // Act
            await groupBL.DeleteGroupAsync(groupIdToDelete);

            // Assert
            groupDalMock.Verify(m => m.DeleteGroupAsync(groupIdToDelete), Times.Once);
        }

        [Fact]
        public async Task DeleteGroupAsync_NonExistingGroupId_DoesNotThrowException()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();

            var groupBL = new GroupBL(groupDalMock.Object, groupMemberDalMock.Object);

            var nonExistingGroupId = 999;

            groupDalMock.Setup(m => m.DeleteGroupAsync(nonExistingGroupId)).Returns(Task.CompletedTask);

            // Act and Assert
            await groupBL.DeleteGroupAsync(nonExistingGroupId); // No exception should be thrown
        }

        [Fact]
        public async Task GetGroupDetailsAsync_ExistingGroupId_ReturnsGroupDetails()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();

            var groupBL = new GroupBL(groupDalMock.Object, groupMemberDalMock.Object);

            var expectedGroup = new Group
            {
                GroupId = 1,
                GroupName = "Test Group",
                GroupAdminId = 1
            };

            groupDalMock.Setup(m => m.GetGroupDetailsAsync(1)).ReturnsAsync(expectedGroup);

            // Act
            var result = await groupBL.GetGroupDetailsAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedGroup.GroupId, result.GroupId);
            Assert.Equal(expectedGroup.GroupName, result.GroupName);
        }

        [Fact]
        public async Task GetGroupDetailsAsync_NonExistingGroupId_ReturnsNull()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();

            var groupBL = new GroupBL(groupDalMock.Object, groupMemberDalMock.Object);

            groupDalMock.Setup(m => m.GetGroupDetailsAsync(It.IsAny<int>())).ReturnsAsync((Group)null);

            // Act
            var result = await groupBL.GetGroupDetailsAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllGroups_ReturnsAllGroups()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();

            var groupBL = new GroupBL(groupDalMock.Object, groupMemberDalMock.Object);

            var expectedGroups = new List<Group>
            {
                new Group { GroupId = 1, GroupName = "Group 1", GroupAdminId = 1 },
                new Group { GroupId = 2, GroupName = "Group 2", GroupAdminId = 2 }
            };

            groupDalMock.Setup(m => m.GetAllGroups()).ReturnsAsync(expectedGroups);

            // Act
            var result = await groupBL.GetAllGroups();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedGroups.Count, result.Count());
            foreach (var expectedGroup in expectedGroups)
            {
                Assert.Contains(result, g => g.GroupId == expectedGroup.GroupId);
            }
        }

        [Fact]
        public async Task GetAllGroups_EmptyList_ReturnsEmptyCollection()
        {
            // Arrange
            var groupDalMock = new Mock<IGroupDAL>();
            groupDalMock.Setup(x => x.GetAllGroups()).ReturnsAsync(new List<Group>());

            var groupBL = new GroupBL(groupDalMock.Object, It.IsAny<IGroupMemberDAL>());

            // Act
            var result = await groupBL.GetAllGroups();

            // Assert
            Assert.Empty(result);
        }
    }
}
