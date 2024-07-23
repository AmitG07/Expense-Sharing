using BusinessObjectLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IExpenseSplitBL
    {
        Task<ExpenseSplit> AddExpenseSplitAsync(ExpenseSplit expenseSplit);
        IEnumerable<ExpenseSplit> GetExpenseSplitsByExpenseId(int expenseId);
        void UpdateExpenseSplit(ExpenseSplit expenseSplit);
        void DeleteExpenseSplit(int expenseSplitId);
    }
}
