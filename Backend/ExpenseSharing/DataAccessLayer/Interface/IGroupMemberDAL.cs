using BusinessObjectLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IGroupMemberDAL
    {
        Task<GroupMember> AddMemberToGroupAsync(GroupMember groupMember);
        Task<User?> GetUserByIdAsync(int userId);
        Task<Group?> GetGroupByIdAsync(int groupId);
        Task<GroupMember?> GetGroupMemberByUserIdAndGroupIdAsync(int userId, int groupId);

        Task<GroupMember> UpdateGroupMemberAsync(GroupMember groupMember);
        Task<GroupMember?> GetGroupMemberByIdAsync(int groupMemberId);
        Task<List<GroupMember>> GetGroupMembersByGroupIdAsync(int groupId);
        Task<List<GroupMember>> GetGroupMembersByUserIdAsync(int userId);
    }
}
