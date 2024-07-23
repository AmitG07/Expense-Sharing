using BusinessLayer.Interface;
using BusinessObjectLayer;
using DataAccessLayer.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class GroupMemberBL : IGroupMemberBL
    {
        private readonly IGroupMemberDAL _groupMemberDal;

        public GroupMemberBL(IGroupMemberDAL groupMemberDal)
        {
            _groupMemberDal = groupMemberDal;
        }

        public async Task<GroupMember> AddMemberToGroupAsync(GroupMember groupMember)
        {
            groupMember.User = await _groupMemberDal.GetUserByIdAsync(groupMember.UserId);
            groupMember.Group = await _groupMemberDal.GetGroupByIdAsync(groupMember.GroupId);

            if (groupMember.User == null || groupMember.Group == null)
            {
                throw new InvalidOperationException("User or Group not found");
            }

            // Check if the user is already a member of the group
            var existingMember = await _groupMemberDal.GetGroupMemberByUserIdAndGroupIdAsync(groupMember.UserId, groupMember.GroupId);
            if (existingMember != null)
            {
                throw new InvalidOperationException("User is already a member of the group");
            }

            return await _groupMemberDal.AddMemberToGroupAsync(groupMember);
        }

        public Task<GroupMember?> GetGroupMemberByIdAsync(int groupMemberId)
        {
            return _groupMemberDal.GetGroupMemberByIdAsync(groupMemberId);
        }

        public Task<List<GroupMember>> GetGroupMembersByGroupIdAsync(int groupId)
        {
            return _groupMemberDal.GetGroupMembersByGroupIdAsync(groupId);
        }

        public Task<List<GroupMember>> GetGroupMembersByUserIdAsync(int userId)
        {
            return _groupMemberDal.GetGroupMembersByUserIdAsync(userId);
        }
    }
}
