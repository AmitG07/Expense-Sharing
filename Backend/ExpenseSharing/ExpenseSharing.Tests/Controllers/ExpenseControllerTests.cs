using BusinessLayer.Interface;
using BusinessObjectLayer;
using ExpenseSharing.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseSharing.Tests.Controllers
{
    public class ExpenseControllerTests
    {
        private readonly Mock<IExpenseBL> _expenseBlMock;
        private Mock<IExpenseBL> expenseBlMock;
        private readonly Mock<IGroupMemberBL> _groupMemberBlMock;
        private readonly Mock<IUserBL> _userBlMock;
        private readonly Mock<ILogger<ExpenseController>> _loggerMock;
        private readonly ExpenseController _controller;

        public ExpenseControllerTests()
        {
            _expenseBlMock = new Mock<IExpenseBL>();
            expenseBlMock = new Mock<IExpenseBL>();
            _groupMemberBlMock = new Mock<IGroupMemberBL>();
            _userBlMock = new Mock<IUserBL>();
            _loggerMock = new Mock<ILogger<ExpenseController>>();
            _controller = new ExpenseController(_loggerMock.Object, _expenseBlMock.Object, _groupMemberBlMock.Object, _userBlMock.Object);
        }

        [Fact]
        public async Task CreateExpense_ValidExpense_ReturnsOkResult()
        {
            // Arrange
            var expense = new Expense { ExpenseId = 1, GroupId = 1, ExpenseAmount = 100 };

            _expenseBlMock.Setup(x => x.CreateExpenseAsync(expense)).ReturnsAsync(expense);

            // Act
            var result = await _controller.CreateExpense(expense);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedExpense = Assert.IsType<Expense>(okResult.Value);
            Assert.Equal(expense.ExpenseId, returnedExpense.ExpenseId);
        }

        [Fact]
        public async Task CreateExpense_InvalidExpense_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Amount", "Required");

            // Act
            var result = await _controller.CreateExpense(new Expense());

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseDict = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(responseDict.ContainsKey("Amount"));
        }

        [Fact]
        public void GetExpenseByExpenseId_ExistingExpenseId_ReturnsOkResult()
        {
            // Arrange
            var expenseId = 1;
            var expense = new Expense { ExpenseId = expenseId, GroupId = 1, ExpenseAmount = 100 };

            _expenseBlMock.Setup(x => x.GetExpenseByExpenseId(expenseId)).Returns(expense);

            // Act
            var result = _controller.GetExpenseByExpenseId(expenseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedExpense = Assert.IsType<Expense>(okResult.Value);
            Assert.Equal(expenseId, returnedExpense.ExpenseId);
        }

        [Fact]
        public void GetExpenseByExpenseId_NonExistingExpenseId_ReturnsNotFound()
        {
            // Arrange
            var expenseId = 1;

            _expenseBlMock.Setup(x => x.GetExpenseByExpenseId(expenseId)).Returns((Expense)null);

            // Act
            var result = _controller.GetExpenseByExpenseId(expenseId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetExpensesByGroupId_ExistingGroupId_ReturnsOkResult()
        {
            // Arrange
            var groupId = 1;
            var expenses = new List<Expense>
            {
                new Expense { ExpenseId = 1, GroupId = groupId, ExpenseAmount = 100 },
                new Expense { ExpenseId = 2, GroupId = groupId, ExpenseAmount = 200 }
            };

            _expenseBlMock.Setup(x => x.GetExpensesByGroupId(groupId)).Returns(expenses);

            // Act
            var result = _controller.GetExpensesByGroupId(groupId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedExpenses = Assert.IsAssignableFrom<IEnumerable<Expense>>(okResult.Value);
            Assert.Equal(2, returnedExpenses.Count());
        }

        [Fact]
        public void GetExpensesByGroupId_NonExistingGroupId_ReturnsNotFound()
        {
            // Arrange
            var groupId = 1;

            _expenseBlMock.Setup(x => x.GetExpensesByGroupId(groupId)).Returns((List<Expense>)null);

            // Act
            var result = _controller.GetExpensesByGroupId(groupId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void UpdateExpense_ValidExpense_ReturnsOkResult()
        {
            // Arrange
            var expenseId = 1;
            var expense = new Expense { ExpenseId = expenseId, GroupId = 1, ExpenseAmount = 100 };

            expenseBlMock.Setup(x => x.UpdateExpense(It.IsAny<Expense>())).Verifiable();

            // Act
            var result = _controller.UpdateExpense(expenseId, expense);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void UpdateExpense_InvalidExpense_ReturnsBadRequest()
        {
            // Arrange
            var expenseId = 1;
            var invalidExpense = new Expense { ExpenseId = 2, GroupId = 1, ExpenseAmount = 100 };

            // Act
            var result = _controller.UpdateExpense(expenseId, invalidExpense);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);

            // Create a template anonymous type for comparison
            var expectedResponse = new { message = "Expense ID mismatch or invalid request body." };
        }

        [Fact]
        public async Task SettleExpense_ValidExpense_ReturnsOkResult()
        {
            // Arrange
            var expenseId = 1;
            var groupId = 1;
            var expense = new Expense { ExpenseId = expenseId, GroupId = groupId, ExpenseAmount = 100, IsSettled = false };

            // Mocking dependencies
            _expenseBlMock.Setup(x => x.GetExpenseByExpenseId(expenseId)).Returns(expense);
            _expenseBlMock.Setup(x => x.GetExpensesByGroupId(groupId)).Returns(new List<Expense> { expense });
            _groupMemberBlMock.Setup(x => x.GetGroupMembersByGroupIdAsync(groupId)).ReturnsAsync(new List<GroupMember>
            {
                new GroupMember { UserId = 1, GroupId = groupId, UserExpense = 50 },
                new GroupMember { UserId = 2, GroupId = groupId, UserExpense = 50 }
            });

            // Act
            var result = await _controller.SettleExpense(expenseId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task SettleExpense_ExpenseNotFound_ReturnsNotFound()
        {
            // Arrange
            var expenseId = 1;
            _expenseBlMock.Setup(x => x.GetExpenseByExpenseId(expenseId)).Returns((Expense)null);

            // Act
            var result = await _controller.SettleExpense(expenseId);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result); // Change to NotFoundObjectResult
        }

        [Fact]
        public async Task DeleteExpense_ExistingExpenseId_ReturnsOkResult()
        {
            // Arrange
            var expenseId = 1;
            _expenseBlMock.Setup(x => x.GetExpenseByExpenseId(expenseId)).Returns(new Expense());
            _expenseBlMock.Setup(x => x.DeleteExpense(expenseId)); 

            // Act
            var result = await _controller.DeleteExpense(expenseId);

            // Assert
            Assert.IsType<OkResult>(result);
            _expenseBlMock.Verify(x => x.DeleteExpense(expenseId), Times.Once);
        }

        [Fact]
        public async Task DeleteExpense_ExpenseNotFound_ReturnsNotFound()
        {
            // Arrange
            var expenseId = 1;

            _expenseBlMock.Setup(x => x.DeleteExpense(expenseId)); 

            // Act
            var result = await _controller.DeleteExpense(expenseId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
