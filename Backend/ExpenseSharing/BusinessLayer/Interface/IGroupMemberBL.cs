using BusinessObjectLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IGroupMemberBL
    {
        Task<GroupMember> AddMemberToGroupAsync(GroupMember groupMember);
        Task<GroupMember?> GetGroupMemberByIdAsync(int groupMemberId);
        Task<List<GroupMember>> GetGroupMembersByGroupIdAsync(int groupId);
        Task<List<GroupMember>> GetGroupMembersByUserIdAsync(int userId);
    }
}
