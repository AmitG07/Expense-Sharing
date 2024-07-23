using BusinessLayer.Interface;
using BusinessObjectLayer;
using DataAccessLayer;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class ExpenseBL : IExpenseBL
    {
        private readonly ILogger<ExpenseBL> _logger;
        private readonly IExpenseDAL _expenseDal;
        private readonly IGroupMemberDAL _groupMemberDal;
        private readonly IExpenseSplitDAL _expenseSplitDal;
        private readonly ApplicationDbContext _context;

        public ExpenseBL(ILogger<ExpenseBL> logger, IExpenseDAL expenseDal, IGroupMemberDAL groupMemberDal, IExpenseSplitDAL expenseSplitDal, ApplicationDbContext context)
        {
            _logger = logger;
            _expenseDal = expenseDal;
            _groupMemberDal = groupMemberDal;
            _expenseSplitDal = expenseSplitDal;
            _context = context;
        }

        public async Task<Expense> CreateExpenseAsync(Expense expense)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var groupMembers = await _groupMemberDal.GetGroupMembersByGroupIdAsync(expense.GroupId);

                var isValidMember = groupMembers.Any(gm => gm.UserId == expense.PaidByUserId);
                if (!isValidMember)
                {
                    throw new InvalidOperationException("User is not a member of the specified group.");
                }

                // Create the expense
                _context.Expenses.Add(expense);
                await _context.SaveChangesAsync();

                // Update GivenAmount for PaidByUserId in GroupMembers
                var paidByMember = groupMembers.First(gm => gm.UserId == expense.PaidByUserId);
                paidByMember.GivenAmount += expense.ExpenseAmount;
                await _groupMemberDal.UpdateGroupMemberAsync(paidByMember);

                // Calculate split amount per member
                double splitAmount = expense.ExpenseAmount / groupMembers.Count;

                // Create ExpenseSplit entries for each group member except PaidByUserId
                foreach (var member in groupMembers)
                {
                    if (member.UserId != expense.PaidByUserId)
                    {
                        var expenseSplit = new ExpenseSplit
                        {
                            ExpenseId = expense.ExpenseId, // Ensure ExpenseId is set correctly
                            SplitWithUserId = member.UserId,
                            SplitAmount = splitAmount
                        };
                        await _expenseSplitDal.AddExpenseSplitAsync(expenseSplit);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return expense;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred while creating the expense.");
                throw new InvalidOperationException("Failed to create the expense.", ex);
            }
        }

        public Expense GetExpenseByExpenseId(int expenseId)
        {
            return _expenseDal.GetExpenseByExpenseId(expenseId);
        }

        public IEnumerable<Expense> GetExpensesByGroupId(int groupId)
        {
            return _expenseDal.GetExpensesByGroupId(groupId);
        }

        public void DeleteExpense(int expenseId)
        {
            var expense = _context.Expenses.Find(expenseId);
            if (expense != null)
            {
                _context.Expenses.Remove(expense);
                _context.SaveChanges();
            }
        }

        public bool UpdateExpenseSettledStatus(int expenseId, bool isSettled)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var expense = _context.Expenses
                    .Include(e => e.ExpenseSplit)
                    .FirstOrDefault(e => e.ExpenseId == expenseId);

                if (expense == null)
                {
                    return false; // Expense not found
                }

                // Update IsSettled in Expense
                expense.IsSettled = isSettled;

                // Update IsSettled in ExpenseSplits
                foreach (var split in expense.ExpenseSplit)
                {
                    _context.ExpenseSplits.Update(split);

                    // Update GroupMember TakenAmount and GivenAmount
                    var groupMemberPaidBy = _context.GroupMembers
                        .FirstOrDefault(gm => gm.UserId == expense.PaidByUserId && gm.GroupId == expense.GroupId);

                    if (groupMemberPaidBy != null)
                    {
                        groupMemberPaidBy.TakenAmount += split.SplitAmount;
                        _context.GroupMembers.Update(groupMemberPaidBy);
                    }

                    var groupMemberSplitWith = _context.GroupMembers
                        .FirstOrDefault(gm => gm.UserId == split.SplitWithUserId && gm.GroupId == expense.GroupId);

                    if (groupMemberSplitWith != null)
                    {
                        groupMemberSplitWith.GivenAmount += split.SplitAmount;
                        _context.GroupMembers.Update(groupMemberSplitWith);
                    }
                }

                _context.SaveChanges();
                transaction.Commit();

                return true; // Successfully updated
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "An error occurred while updating the expense settled status.");
                throw new InvalidOperationException("Failed to update the expense settled status.", ex);
            }
        }

        public void UpdateExpense(Expense expense)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var currentExpense = _context.Expenses
                    .Include(e => e.ExpenseSplit)
                    .FirstOrDefault(e => e.ExpenseId == expense.ExpenseId);

                if (currentExpense == null)
                {
                    throw new InvalidOperationException("Expense not found.");
                }

                // Update properties of currentExpense with values from expense
                currentExpense.GroupId = expense.GroupId;
                currentExpense.Description = expense.Description;
                currentExpense.ExpenseAmount = expense.ExpenseAmount;
                currentExpense.PaidByUserId = expense.PaidByUserId;
                currentExpense.ExpenseCreatedAt = expense.ExpenseCreatedAt;
                currentExpense.IsSettled = expense.IsSettled;

                // Save changes to Expense entity
                _context.Expenses.Update(currentExpense);
                _context.SaveChanges();

                // Perform additional actions based on business logic if needed
                if (currentExpense.IsSettled)
                {
                    UpdateSettledExpenseDetails(currentExpense);
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "An error occurred while updating the expense.");
                throw new InvalidOperationException("Failed to update the expense.", ex);
            }
        }

        public void UpdateExpenseDetails(int expenseId, Expense updatedExpense)
        {
            using var transaction = _context.Database.BeginTransaction();

            try
            {
                var existingExpense = _context.Expenses
                    .Include(e => e.ExpenseSplit)
                    .FirstOrDefault(e => e.ExpenseId == expenseId);

                if (existingExpense == null)
                {
                    throw new InvalidOperationException("Expense not found.");
                }

                // Update properties of existingExpense with values from updatedExpense
                existingExpense.GroupId = updatedExpense.GroupId;
                existingExpense.Description = updatedExpense.Description;
                existingExpense.ExpenseAmount = updatedExpense.ExpenseAmount;
                existingExpense.PaidByUserId = updatedExpense.PaidByUserId;
                existingExpense.ExpenseCreatedAt = updatedExpense.ExpenseCreatedAt;
                existingExpense.IsSettled = updatedExpense.IsSettled;

                // Save changes to Expense entity
                _context.Expenses.Update(existingExpense);
                _context.SaveChanges();

                // Perform additional actions based on business logic if needed
                if (existingExpense.IsSettled)
                {
                    UpdateSettledExpenseDetails(existingExpense);
                }

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "An error occurred while updating the expense.");
                throw new InvalidOperationException("Failed to update the expense.", ex);
            }
        }

        private void UpdateSettledExpenseDetails(Expense expense)
        {
            var groupMembers = _context.GroupMembers
                .Where(gm => gm.GroupId == expense.GroupId)
                .ToList();

            var paidByMember = groupMembers.FirstOrDefault(gm => gm.UserId == expense.PaidByUserId);
            if (paidByMember == null)
            {
                throw new InvalidOperationException("PaidByUserId not found in group members.");
            }

            // Calculate total split amount including PaidByUser's contribution
            double totalSplitAmount = expense.ExpenseSplit.Sum(es => es.SplitAmount);

            // Update PaidByUser's TakenAmount
            paidByMember.TakenAmount += totalSplitAmount;
            paidByMember.UserExpense = paidByMember.GivenAmount - paidByMember.TakenAmount;
            _context.GroupMembers.Update(paidByMember);

            // Update GivenAmount for other group members involved in the split
            foreach (var split in expense.ExpenseSplit)
            {
                var splitWithMember = groupMembers.FirstOrDefault(gm => gm.UserId == split.SplitWithUserId);
                if (splitWithMember != null)
                {
                    splitWithMember.GivenAmount += split.SplitAmount;
                    splitWithMember.UserExpense = splitWithMember.GivenAmount - splitWithMember.TakenAmount;
                    _context.GroupMembers.Update(splitWithMember);
                }
            }

            _context.SaveChanges();
        }
    }
}
