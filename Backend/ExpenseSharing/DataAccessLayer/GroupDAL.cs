using BusinessObjectLayer;
using Dapper;
using DataAccessLayer.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class GroupDAL : IGroupDAL
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public GroupDAL(ApplicationDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        public async Task CreateGroupAsync(Group group)
        {
            _dbContext.Groups.Add(group);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Group>> GetAllGroups()
        {
            return await _dbContext.Groups.ToListAsync();
        }

        public async Task<Group> GetGroupByGroupIdAsync(int groupId)
        {
            return await _dbContext.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
        }

        public async Task<IEnumerable<Group>> GetGroupsByUserIdAsync(int userId)
        {
            return await _dbContext.Groups.Where(g => g.GroupAdminId == userId).ToListAsync();
        }

        public async Task UpdateGroupAsync(Group group)
        {
            _dbContext.Entry(group).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteGroupAsync(int groupId)
        {
            var group = await _dbContext.Groups.FirstOrDefaultAsync(g => g.GroupId == groupId);
            if (group != null)
            {
                _dbContext.Groups.Remove(group);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Group> GetGroupDetailsAsync(int groupId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = @"
                SELECT 
                    g.GroupId, 
                    g.GroupName, 
                    g.Description, 
                    g.CreatedDate, 
                    g.GroupAdminId, 
                    g.TotalMembers, 
                    g.TotalExpense
                FROM 
                    [Groups] g
                WHERE 
                    g.GroupId = @GroupId;";

                var parameters = new { GroupId = groupId };

                var result = await db.QueryAsync<Group>(sqlQuery,parameters);

                var fetchedGroup = result.FirstOrDefault();

                if (fetchedGroup != null)
                {
                    // Fetch group members
                    string sqlGroupMembers = @"
                    SELECT gm.GroupMemberId, gm.GroupId, gm.UserId, gm.UserExpense, gm.GivenAmount, gm.TakenAmount,
                           u.UserId, u.Name, u.EmailId
                    FROM [GroupMembers] gm
                    LEFT JOIN [Users] u ON gm.UserId = u.UserId
                    WHERE gm.GroupId = @GroupId;";

                    var groupMembers = await db.QueryAsync<GroupMember, User, GroupMember>(
                        sqlGroupMembers,
                        (gm, user) =>
                        {
                            gm.User = user;
                            return gm;
                        },
                        parameters,
                        splitOn: "UserId");

                    fetchedGroup.GroupMember = groupMembers.ToList();

                    // Update totalMembers count based on fetched group members
                    fetchedGroup.TotalMembers = fetchedGroup.GroupMember.Count;

                    // Ensure totalMembers does not exceed 10
                    if (fetchedGroup.TotalMembers > 10)
                    {
                        fetchedGroup.TotalMembers = 10;
                    }

                    // Fetch expenses
                    string sqlExpenses = @"
                    SELECT e.ExpenseId, e.PaidByUserId, e.GroupId, e.Description, e.ExpenseAmount, e.IsSettled, e.ExpenseCreatedAt
                    FROM [Expenses] e
                    WHERE e.GroupId = @GroupId;";

                    var expenses = await db.QueryAsync<Expense>(
                        sqlExpenses,
                        parameters);

                    fetchedGroup.Expense = expenses.ToList();

                    // Calculate totalExpense based on fetched expenses
                    fetchedGroup.TotalExpense = fetchedGroup.Expense.Sum(e => e.ExpenseAmount);

                    // Save the updated totalMembers and totalExpense to the database
                    string updateGroupQuery = @"
                    UPDATE [Groups]
                    SET TotalMembers = @TotalMembers,
                        TotalExpense = @TotalExpense
                    WHERE GroupId = @GroupId";

                    await db.ExecuteAsync(updateGroupQuery, new
                    {
                        TotalMembers = fetchedGroup.TotalMembers,
                        TotalExpense = fetchedGroup.TotalExpense,
                        GroupId = groupId
                    });

                    // Fetch expense splits
                    string sqlExpenseSplits = @"
                    SELECT es.ExpenseSplitId, es.ExpenseId, es.SplitWithUserId, es.SplitAmount, es.CreatedAt,
                           u.UserId, u.Name, u.EmailId
                    FROM [ExpenseSplits] es
                    LEFT JOIN [Users] u ON es.SplitWithUserId = u.UserId
                    WHERE es.ExpenseId IN (SELECT e.ExpenseId FROM [Expenses] e WHERE e.GroupId = @GroupId);";

                    foreach (var expense in fetchedGroup.Expense)
                    {
                        var expenseSplits = await db.QueryAsync<ExpenseSplit, User, ExpenseSplit>(
                            sqlExpenseSplits,
                            (es, user) =>
                            {
                                es.User = user;
                                return es;
                            },
                            new { GroupId = groupId },
                            splitOn: "UserId");

                        expense.ExpenseSplit = expenseSplits.ToList();
                    }

                    return fetchedGroup;
                }
                return null;
            }
        }
    }
}
