using Xunit;
using Moq;
using BusinessLayer;
using BusinessLayer.Interface;
using BusinessObjectLayer;
using DataAccessLayer.Interface;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ExpenseSharing.Tests.BusinessLayer
{
    public class ExpenseSplitBLTests
    {
        [Fact]
        public async Task AddExpenseSplitAsync_ValidExpenseSplit_AddsSuccessfully()
        {
            // Arrange
            var mockExpenseSplitDAL = new Mock<IExpenseSplitDAL>();
            var expenseSplitBL = new ExpenseSplitBL(mockExpenseSplitDAL.Object);
            var expenseSplitToAdd = new ExpenseSplit { ExpenseSplitId = 1, ExpenseId = 1, SplitWithUserId = 1, SplitAmount = 50 };

            mockExpenseSplitDAL.Setup(m => m.AddExpenseSplitAsync(It.IsAny<ExpenseSplit>()))
                               .ReturnsAsync(expenseSplitToAdd);

            // Act
            var addedExpenseSplit = await expenseSplitBL.AddExpenseSplitAsync(expenseSplitToAdd);

            // Assert
            Assert.NotNull(addedExpenseSplit);
            Assert.Equal(expenseSplitToAdd.ExpenseSplitId, addedExpenseSplit.ExpenseSplitId);
            Assert.Equal(expenseSplitToAdd.ExpenseId, addedExpenseSplit.ExpenseId);
            Assert.Equal(expenseSplitToAdd.SplitWithUserId, addedExpenseSplit.SplitWithUserId);
            Assert.Equal(expenseSplitToAdd.SplitAmount, addedExpenseSplit.SplitAmount);
        }

        [Fact]
        public async Task AddExpenseSplitAsync_InvalidExpenseSplit_ReturnsNull()
        {
            // Arrange
            var mockExpenseSplitDAL = new Mock<IExpenseSplitDAL>();
            var expenseSplitBL = new ExpenseSplitBL(mockExpenseSplitDAL.Object);
            var invalidExpenseSplit = new ExpenseSplit { ExpenseId = 1, SplitWithUserId = 1, SplitAmount = 50 };

            mockExpenseSplitDAL.Setup(m => m.AddExpenseSplitAsync(It.IsAny<ExpenseSplit>()))
                               .ReturnsAsync((ExpenseSplit)null);

            // Act
            var addedExpenseSplit = await expenseSplitBL.AddExpenseSplitAsync(invalidExpenseSplit);

            // Assert
            Assert.Null(addedExpenseSplit);
        }

        [Fact]
        public void GetExpenseSplitsByExpenseId_ExistingSplits_ReturnsList()
        {
            // Arrange
            var mockExpenseSplitDAL = new Mock<IExpenseSplitDAL>();
            var expenseSplitBL = new ExpenseSplitBL(mockExpenseSplitDAL.Object);
            var expenseId = 1;
            var expenseSplits = new List<ExpenseSplit>
            {
                new ExpenseSplit { ExpenseSplitId = 1, ExpenseId = expenseId, SplitWithUserId = 1, SplitAmount = 50 },
                new ExpenseSplit { ExpenseSplitId = 2, ExpenseId = expenseId, SplitWithUserId = 2, SplitAmount = 50 }
            };

            mockExpenseSplitDAL.Setup(m => m.GetExpenseSplitsByExpenseId(expenseId))
                               .Returns(expenseSplits);

            // Act
            var result = expenseSplitBL.GetExpenseSplitsByExpenseId(expenseId);

            // Assert
            Assert.Equal(expenseSplits.Count, result.Count());
            Assert.Equal(expenseSplits, result);
        }

        [Fact]
        public void GetExpenseSplitsByExpenseId_NoSplits_ReturnsEmpty()
        {
            // Arrange
            var mockExpenseSplitDAL = new Mock<IExpenseSplitDAL>();
            var expenseSplitBL = new ExpenseSplitBL(mockExpenseSplitDAL.Object);
            var expenseId = 1;

            mockExpenseSplitDAL.Setup(m => m.GetExpenseSplitsByExpenseId(expenseId))
                               .Returns(new List<ExpenseSplit>());

            // Act
            var result = expenseSplitBL.GetExpenseSplitsByExpenseId(expenseId);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void UpdateExpenseSplit_ValidExpenseSplit_UpdatesSuccessfully()
        {
            // Arrange
            var mockExpenseSplitDAL = new Mock<IExpenseSplitDAL>();
            var expenseSplitBL = new ExpenseSplitBL(mockExpenseSplitDAL.Object);
            var expenseSplitToUpdate = new ExpenseSplit { ExpenseSplitId = 1, ExpenseId = 1, SplitWithUserId = 1, SplitAmount = 50 };

            // Act
            expenseSplitBL.UpdateExpenseSplit(expenseSplitToUpdate);

            // Assert: No explicit assert for void methods
        }

        [Fact]
        public void UpdateExpenseSplit_NonExistingExpenseSplit_ThrowsException()
        {
            // Arrange
            var mockExpenseSplitDAL = new Mock<IExpenseSplitDAL>();
            var expenseSplitBL = new ExpenseSplitBL(mockExpenseSplitDAL.Object);
            var nonExistingExpenseSplit = new ExpenseSplit { ExpenseSplitId = 999, ExpenseId = 1, SplitWithUserId = 1, SplitAmount = 50 };

            mockExpenseSplitDAL.Setup(m => m.UpdateExpenseSplit(It.IsAny<ExpenseSplit>()))
                               .Throws(new Exception("Expense split not found"));

            // Act and Assert
            Assert.Throws<Exception>(() => expenseSplitBL.UpdateExpenseSplit(nonExistingExpenseSplit));
        }

        [Fact]
        public void DeleteExpenseSplit_ExistingExpenseSplit_DeletesSuccessfully()
        {
            // Arrange
            var mockExpenseSplitDAL = new Mock<IExpenseSplitDAL>();
            var expenseSplitBL = new ExpenseSplitBL(mockExpenseSplitDAL.Object);
            var expenseSplitId = 1;

            // Act
            expenseSplitBL.DeleteExpenseSplit(expenseSplitId);

            // Assert: No explicit assert for void methods
        }

        [Fact]
        public void DeleteExpenseSplit_NonExistingExpenseSplit_ThrowsException()
        {
            // Arrange
            var mockExpenseSplitDAL = new Mock<IExpenseSplitDAL>();
            var expenseSplitBL = new ExpenseSplitBL(mockExpenseSplitDAL.Object);
            var nonExistingExpenseSplitId = 999;

            mockExpenseSplitDAL.Setup(m => m.DeleteExpenseSplit(nonExistingExpenseSplitId))
                               .Throws(new Exception("Expense split not found"));

            // Act and Assert
            Assert.Throws<Exception>(() => expenseSplitBL.DeleteExpenseSplit(nonExistingExpenseSplitId));
        }

    }
}
