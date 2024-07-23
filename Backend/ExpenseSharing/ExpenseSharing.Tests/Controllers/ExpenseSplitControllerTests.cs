using BusinessLayer.Interface;
using BusinessObjectLayer;
using ExpenseSharing.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseSharing.Tests.Controllers
{
    public class ExpenseSplitControllerTests
    {
        private readonly Mock<IExpenseSplitBL> _expenseSplitBlMock;
        private readonly Mock<ILogger<ExpenseSplitController>> _loggerMock;
        private readonly ExpenseSplitController _controller;

        public ExpenseSplitControllerTests()
        {
            _expenseSplitBlMock = new Mock<IExpenseSplitBL>();
            _loggerMock = new Mock<ILogger<ExpenseSplitController>>();
            _controller = new ExpenseSplitController(_loggerMock.Object, _expenseSplitBlMock.Object);
        }

        [Fact]
        public async Task AddExpenseSplit_ValidExpenseSplit_ReturnsOkResult()
        {
            // Arrange
            var expenseSplit = new ExpenseSplit { ExpenseId = 1, SplitWithUserId = 1 };

            _expenseSplitBlMock.Setup(x => x.AddExpenseSplitAsync(expenseSplit))
                .ReturnsAsync(expenseSplit); 

            // Act
            var result = await _controller.AddExpenseSplit(expenseSplit);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var model = Assert.IsType<ExpenseSplit>(okResult.Value);
            Assert.Equal(expenseSplit.ExpenseId, model.ExpenseId);
            Assert.Equal(expenseSplit.SplitWithUserId, model.SplitWithUserId);
        }

        [Fact]
        public async Task AddExpenseSplit_InvalidExpenseSplit_ReturnsBadRequest()
        {
            // Arrange
            var invalidExpenseSplit = new ExpenseSplit(); 

            _expenseSplitBlMock.Setup(x => x.AddExpenseSplitAsync(invalidExpenseSplit))
                .ThrowsAsync(new InvalidOperationException("Invalid expense split"));

            // Act
            var result = await _controller.AddExpenseSplit(invalidExpenseSplit);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var error = Assert.IsType<Dictionary<string, string>>(badRequestResult.Value);
            Assert.Equal("Invalid expense split", error["message"]);
        }

        [Fact]
        public void GetExpenseSplitsByExpenseId_ExistingExpenseId_ReturnsOkResult()
        {
            // Arrange
            var expenseId = 1;
            var expenseSplits = new List<ExpenseSplit>
            {
                new ExpenseSplit { ExpenseId = expenseId, SplitWithUserId = 1 },
                new ExpenseSplit { ExpenseId = expenseId, SplitWithUserId = 2 }
            };

            _expenseSplitBlMock.Setup(x => x.GetExpenseSplitsByExpenseId(expenseId))
                .Returns(expenseSplits); // Mocking the return value

            // Act
            var result = _controller.GetExpenseSplitsByExpenseId(expenseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var models = Assert.IsType<List<ExpenseSplit>>(okResult.Value);
            Assert.Equal(2, models.Count);
        }

        [Fact]
        public void GetExpenseSplitsByExpenseId_NonExistingExpenseId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingExpenseId = 999; // Non-existing expenseId

            _expenseSplitBlMock.Setup(x => x.GetExpenseSplitsByExpenseId(nonExistingExpenseId))
                .Returns((List<ExpenseSplit>)null); // Mocking null return

            // Act
            var result = _controller.GetExpenseSplitsByExpenseId(nonExistingExpenseId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdateExpenseSplit_ValidExpenseSplit_ReturnsOkResult()
        {
            // Arrange
            var expenseSplitId = 1;
            var expenseSplit = new ExpenseSplit { ExpenseSplitId = expenseSplitId, ExpenseId = 1, SplitWithUserId = 1 };

            _expenseSplitBlMock.Setup(x => x.UpdateExpenseSplit(expenseSplit))
                .Verifiable();

            // Act
            var result = _controller.UpdateExpenseSplit(expenseSplitId, expenseSplit);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var message = okResult.Value.GetType().GetProperty("message").GetValue(okResult.Value, null);
            Assert.Equal("Expense Split updated successfully.", message);
        }

        [Fact]
        public void UpdateExpenseSplit_InvalidExpenseSplit_ReturnsBadRequest()
        {
            // Arrange
            var expenseSplitId = 1;
            var invalidExpenseSplit = new ExpenseSplit { ExpenseSplitId = 2, ExpenseId = 1, SplitWithUserId = 1 };

            // Act
            var result = _controller.UpdateExpenseSplit(expenseSplitId, invalidExpenseSplit);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var error = badRequestResult.Value.GetType().GetProperty("message").GetValue(badRequestResult.Value, null);
            Assert.Equal("Expense Split ID mismatch or invalid request body.", error);
        }

        [Fact]
        public void DeleteExpenseSplit_ExistingExpenseSplitId_ReturnsOkResult()
        {
            // Arrange
            var expenseSplitId = 1;

            _expenseSplitBlMock.Setup(x => x.DeleteExpenseSplit(expenseSplitId))
                .Verifiable(); // Mocking the delete call

            // Act
            var result = _controller.DeleteExpenseSplit(expenseSplitId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void DeleteExpenseSplit_NonExistingExpenseSplitId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingExpenseSplitId = 999;
            _expenseSplitBlMock.Setup(x => x.DeleteExpenseSplit(nonExistingExpenseSplitId))
                .Throws(new InvalidOperationException("Expense split not found"));

            // Act
            var result = _controller.DeleteExpenseSplit(nonExistingExpenseSplitId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }
    }
}
