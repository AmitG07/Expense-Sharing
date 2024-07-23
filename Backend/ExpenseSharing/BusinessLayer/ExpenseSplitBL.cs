using BusinessLayer.Interface;
using BusinessObjectLayer;
using DataAccessLayer.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ExpenseSplitBL : IExpenseSplitBL
    {
        private readonly IExpenseSplitDAL _expenseSplitDal;

        public ExpenseSplitBL(IExpenseSplitDAL expenseSplitDal)
        {
            _expenseSplitDal = expenseSplitDal;
        }

        public async Task<ExpenseSplit> AddExpenseSplitAsync(ExpenseSplit expenseSplit)
        {
            return await _expenseSplitDal.AddExpenseSplitAsync(expenseSplit);
        }

        public IEnumerable<ExpenseSplit> GetExpenseSplitsByExpenseId(int expenseId)
        {
            return _expenseSplitDal.GetExpenseSplitsByExpenseId(expenseId);
        }

        public void UpdateExpenseSplit(ExpenseSplit expenseSplit)
        {
            _expenseSplitDal.UpdateExpenseSplit(expenseSplit);
        }

        public void DeleteExpenseSplit(int expenseSplitId)
        {
            _expenseSplitDal.DeleteExpenseSplit(expenseSplitId);
        }
    }
}
