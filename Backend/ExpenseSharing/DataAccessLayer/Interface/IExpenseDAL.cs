using BusinessObjectLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IExpenseDAL
    {
        Task<Expense> CreateExpenseAsync(Expense expense);
        Expense GetExpenseByExpenseId(int expenseId);
        IEnumerable<Expense> GetExpensesByGroupId(int groupId);
        void UpdateExpense(Expense expense);
        void DeleteExpense(int expenseId);
    }
}
