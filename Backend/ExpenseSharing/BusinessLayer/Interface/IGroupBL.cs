using BusinessObjectLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IGroupBL
    {
        Task CreateGroupAsync(Group group);
        Task<Group> GetGroupByGroupIdAsync(int groupId);
        Task<IEnumerable<Group>> GetGroupsByUserIdAsync(int userId);
        Task UpdateGroupAsync(Group group);
        Task DeleteGroupAsync(int groupId);
        Task<Group> GetGroupDetailsAsync(int groupId);
        Task<IEnumerable<Group>> GetAllGroups();
    }
}
