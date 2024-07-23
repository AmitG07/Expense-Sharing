using BusinessObjectLayer;
using DataAccessLayer;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseSharing.Tests.DataAccessLayer
{
    public class ExpenseDALTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;
        private ApplicationDbContext _context;

        public ExpenseDALTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context = new ApplicationDbContext(_options);

            // Seed initial data
            var groups = new List<Group>
            {
                new Group { GroupId = 1 },
                new Group { GroupId = 2 },
            };

            _context.Groups.AddRange(groups);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Fact]
        public async Task CreateExpenseAsync_ValidExpense_CreatesExpense()
        {
            // Arrange
            var initialCount = _context.Expenses.Count(); // Get initial count of expenses

            var expense = new Expense { ExpenseId = 1, GroupId = 1, ExpenseAmount = 100 };

            var expenseDAL = new ExpenseDAL(_context);

            // Act
            var result = await expenseDAL.CreateExpenseAsync(expense);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ExpenseId);
            Assert.Equal(1, result.GroupId); 
            Assert.Equal(100, result.ExpenseAmount);

            // Additional Debugging
            var finalCount = _context.Expenses.Count();
            Assert.Equal(initialCount + 1, finalCount);
        }

        [Fact]
        public async Task CreateExpenseAsync_InvalidGroup_ReturnsNull()
        {
            // Arrange
            var initialCount = _context.Expenses.Count(); // Get initial count of expenses

            var expense = new Expense { ExpenseId = 2, GroupId = 999, ExpenseAmount = 100 };

            var expenseDAL = new ExpenseDAL(_context);

            // Act
            var result = await expenseDAL.CreateExpenseAsync(expense);

            // Assert
            Assert.Null(result); 

            // Additional Debugging
            var finalCount = _context.Expenses.Count(); 
            Assert.Equal(initialCount, finalCount); 
        }

        [Fact]
        public void GetExpenseByExpenseId_ExistingExpenseId_ReturnsExpense()
        {
            // Arrange
            var expense = new Expense { ExpenseId = 1, GroupId = 1, ExpenseAmount = 100 };

            using (var context = new ApplicationDbContext(_options))
            {
                context.Expenses.Add(expense);
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(_options))
            {
                var expenseDAL = new ExpenseDAL(context);

                // Act
                var result = expenseDAL.GetExpenseByExpenseId(expense.ExpenseId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(expense.ExpenseId, result.ExpenseId);
            }
        }

        [Fact]
        public void GetExpenseByExpenseId_NonExistingExpenseId_ReturnsNull()
        {
            // Arrange
            var expenseId = 999;

            using (var context = new ApplicationDbContext(_options))
            {
                var expenseDAL = new ExpenseDAL(context);

                // Act
                var result = expenseDAL.GetExpenseByExpenseId(expenseId);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public void GetExpensesByGroupId_ExistingGroupId_ReturnsExpenses()
        {
            // Arrange
            var groupId = 1;
            var expenses = new List<Expense>
            {
                new Expense { ExpenseId = 1, GroupId = groupId, ExpenseAmount = 100 },
                new Expense { ExpenseId = 2, GroupId = groupId, ExpenseAmount = 200 }
            };

            _context.Expenses.AddRange(expenses);
            _context.SaveChanges();

            var expenseDAL = new ExpenseDAL(_context);

            // Act
            var result = expenseDAL.GetExpensesByGroupId(groupId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expenses.Count, result.Count());
            Assert.True(result.All(e => e.GroupId == groupId));
        }

        [Fact]
        public void GetExpensesByGroupId_NonExistingGroupId_ReturnsEmptyList()
        {
            // Arrange
            var groupId = 999;

            using (var context = new ApplicationDbContext(_options))
            {
                var expenseDAL = new ExpenseDAL(context);

                // Act
                var result = expenseDAL.GetExpensesByGroupId(groupId).ToList();

                // Assert
                Assert.NotNull(result);
                Assert.Empty(result);
            }
        }

        [Fact]
        public void DeleteExpense_ExistingExpenseId_DeletesExpense()
        {
            // Arrange
            var expense = new Expense { ExpenseId = 1, GroupId = 1, ExpenseAmount = 100 };

            using (var context = new ApplicationDbContext(_options))
            {
                context.Expenses.Add(expense);
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(_options))
            {
                var expenseDAL = new ExpenseDAL(context);

                // Act
                expenseDAL.DeleteExpense(expense.ExpenseId);

                // Assert
                var deletedExpense = context.Expenses.Find(expense.ExpenseId);
                Assert.Null(deletedExpense);
            }
        }

        [Fact]
        public void DeleteExpense_NonExistingExpenseId_DoesNothing()
        {
            // Arrange
            var expenseId = 999;

            using (var context = new ApplicationDbContext(_options))
            {
                var expenseDAL = new ExpenseDAL(context);

                // Act
                expenseDAL.DeleteExpense(expenseId);

                // Assert
                Assert.Empty(context.Expenses);
            }
        }
    }
}
