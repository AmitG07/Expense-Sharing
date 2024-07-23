using BusinessObjectLayer;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class GroupMemberDAL : IGroupMemberDAL
    {
        private readonly ApplicationDbContext _context;

        public GroupMemberDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<GroupMember> AddMemberToGroupAsync(GroupMember groupMember)
        {
            _context.GroupMembers.Add(groupMember);
            await _context.SaveChangesAsync();

            // Update totalMembers count in the Groups table
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == groupMember.GroupId);
            if (group != null)
            {
                group.TotalMembers = await _context.GroupMembers.CountAsync(gm => gm.GroupId == group.GroupId);
                await _context.SaveChangesAsync();
            }

            return groupMember;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task<Group?> GetGroupByIdAsync(int groupId)
        {
            return await _context.Groups.FindAsync(groupId);
        }

        public async Task<GroupMember> UpdateGroupMemberAsync(GroupMember groupMember)
        {
            _context.Entry(groupMember).State = EntityState.Modified;
            groupMember.UserExpense = groupMember.GivenAmount - groupMember.TakenAmount; // Update UserExpense
            await _context.SaveChangesAsync();

            return groupMember;
        }

        public async Task<GroupMember?> GetGroupMemberByIdAsync(int groupMemberId)
        {
            return await _context.GroupMembers.FindAsync(groupMemberId);
        }

        public async Task<List<GroupMember>> GetGroupMembersByGroupIdAsync(int groupId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.GroupId == groupId)
                .Include(gm => gm.User) // Include User navigation property
                .ToListAsync();
        }

        public async Task<List<GroupMember>> GetGroupMembersByUserIdAsync(int userId)
        {
            return await _context.GroupMembers
                .Where(gm => gm.UserId == userId && gm.Group.GroupAdminId != userId)
                .Include(gm => gm.Group) // Include Group navigation property
                .ToListAsync();
        }

        public async Task<GroupMember?> GetGroupMemberByUserIdAndGroupIdAsync(int userId, int groupId)
        {
            return await _context.GroupMembers
                .FirstOrDefaultAsync(gm => gm.UserId == userId && gm.GroupId == groupId);
        }
    }
}
