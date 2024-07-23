using BusinessObjectLayer;
using DataAccessLayer.Interface;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class ExpenseSplitDAL : IExpenseSplitDAL
    {
        private readonly ApplicationDbContext _context;

        public ExpenseSplitDAL(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ExpenseSplit> AddExpenseSplitAsync(ExpenseSplit expenseSplit)
        {
            _context.ExpenseSplits.Add(expenseSplit);
            await _context.SaveChangesAsync();
            return expenseSplit;
        }

        public IEnumerable<ExpenseSplit> GetExpenseSplitsByExpenseId(int expenseId)
        {
            return _context.ExpenseSplits.Where(es => es.ExpenseId == expenseId).ToList();
        }

        public void UpdateExpenseSplit(ExpenseSplit expenseSplit)
        {
            _context.ExpenseSplits.Update(expenseSplit);
            _context.SaveChanges();
        }

        public void DeleteExpenseSplit(int expenseSplitId)
        {
            var expenseSplit = _context.ExpenseSplits.Find(expenseSplitId);
            if (expenseSplit != null)
            {
                _context.ExpenseSplits.Remove(expenseSplit);
                _context.SaveChanges();
            }
        }
    }
}