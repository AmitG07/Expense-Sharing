using BusinessObjectLayer;

namespace BusinessLayer.Interface
{
    public interface IExpenseBL
    {
        Task<Expense> CreateExpenseAsync(Expense expense);
        Expense GetExpenseByExpenseId(int id);
        IEnumerable<Expense> GetExpensesByGroupId(int groupId);
        void UpdateExpense(Expense expense);
        void DeleteExpense(int expenseId);
        bool UpdateExpenseSettledStatus(int expenseId, bool isSettled);
        void UpdateExpenseDetails(int expenseId, Expense updatedExpense);
    }
}
