using BusinessObjectLayer;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ExpenseDAL : IExpenseDAL
    {
        private readonly ApplicationDbContext _context;

        public ExpenseDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Expense> CreateExpenseAsync(Expense expense)
        {
            // Check if the group exists
            var group_exists = await _context.Groups.FindAsync(expense.GroupId);
            if (group_exists == null)
            {
                return null; 
            }
            
            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();

            // Update the total expense in the Group table
            var group = await _context.Groups.FirstOrDefaultAsync(g => g.GroupId == expense.GroupId);
            if (group != null)
            {
                group.TotalExpense = await _context.Expenses
                    .Where(e => e.GroupId == expense.GroupId)
                    .SumAsync(e => e.ExpenseAmount);
                await _context.SaveChangesAsync();
            }

            return expense;
        }

        public Expense GetExpenseByExpenseId(int expenseId)
        {
            return _context.Expenses.FirstOrDefault(e => e.ExpenseId == expenseId);
        }

        public IEnumerable<Expense> GetExpensesByGroupId(int groupId)
        {
            return _context.Expenses
                           .Include(e => e.ExpenseSplit)
                           .Where(e => e.GroupId == groupId)
                           .ToList();
        }

        public void UpdateExpense(Expense expense)
        {
            _context.Entry(expense).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void DeleteExpense(int expenseId)
        {
            var expense = _context.Expenses.FirstOrDefault(e => e.ExpenseId == expenseId);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                _context.SaveChanges();
            }
        }
    }
}
