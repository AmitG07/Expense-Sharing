using BusinessLayer.Interface;
using BusinessObjectLayer;
using DataAccessLayer.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class GroupBL : IGroupBL
    {
        private readonly IGroupDAL _groupDal;
        private readonly IGroupMemberDAL _groupMemberDal;

        public GroupBL(IGroupDAL groupDal, IGroupMemberDAL groupMemberDal)
        {
            _groupDal = groupDal;
            _groupMemberDal = groupMemberDal;
        }

        public async Task CreateGroupAsync(Group group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            // Create the group using data access layer
            await _groupDal.CreateGroupAsync(group);

            // After creating the group, add the group admin as a member
            GroupMember groupMember = new GroupMember
            {
                GroupId = group.GroupId,
                UserId = group.GroupAdminId, // Add the admin user ID as a member
                UserExpense = 0, // Set initial values as needed
                GivenAmount = 0,
                TakenAmount = 0
            };

            await _groupMemberDal.AddMemberToGroupAsync(groupMember); // Await this call if necessary
        }

        public async Task<Group> GetGroupByGroupIdAsync(int groupId)
        {
            return await _groupDal.GetGroupByGroupIdAsync(groupId);
        }

        public async Task<IEnumerable<Group>> GetGroupsByUserIdAsync(int userId)
        {
            return await _groupDal.GetGroupsByUserIdAsync(userId);
        }

        public async Task UpdateGroupAsync(Group group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            await _groupDal.UpdateGroupAsync(group);
        }

        public async Task DeleteGroupAsync(int groupId)
        {
            await _groupDal.DeleteGroupAsync(groupId);
        }

        public async Task<Group> GetGroupDetailsAsync(int groupId)
        {
            return await _groupDal.GetGroupDetailsAsync(groupId);
        }

        public async Task<IEnumerable<Group>> GetAllGroups()
        {
            return await _groupDal.GetAllGroups();
        }
    }
}
