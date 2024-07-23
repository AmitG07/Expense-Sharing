using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLayer;
using BusinessLayer.Interface;
using BusinessObjectLayer;
using DataAccessLayer;
using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ExpenseSharing.Tests.BusinessLayer
{
    public class ExpenseBLTests
    {
        private readonly Mock<ILogger<ExpenseBL>> _loggerMock = new Mock<ILogger<ExpenseBL>>();
        private readonly Mock<IExpenseDAL> _expenseDalMock = new Mock<IExpenseDAL>();
        private readonly Mock<IGroupMemberDAL> _groupMemberDalMock = new Mock<IGroupMemberDAL>();
        private readonly Mock<IExpenseSplitDAL> _expenseSplitDalMock = new Mock<IExpenseSplitDAL>();
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public ExpenseBLTests()
        {
            // Initialize DbContextOptions with an in-memory database provider
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;
        }

        [Fact]
        public async Task CreateExpenseAsync_ValidExpense_CreatesExpense()
        {
            // Arrange
            var expense = new Expense
            {
                ExpenseId = 1,
                GroupId = 1,
                PaidByUserId = 1,
                ExpenseAmount = 100.0,
                ExpenseCreatedAt = DateTime.UtcNow,
                IsSettled = false
            };

            var groupMembers = new List<GroupMember>
            {
                new GroupMember { UserId = 1, GroupId = 1, GivenAmount = 0, TakenAmount = 0 },
                new GroupMember { UserId = 2, GroupId = 1, GivenAmount = 0, TakenAmount = 0 }
            };

            // Mock IGroupMemberDAL
            _groupMemberDalMock.Setup(mock => mock.GetGroupMembersByGroupIdAsync(1))
                               .ReturnsAsync(groupMembers);

            // Mock IExpenseSplitDAL
            _expenseSplitDalMock.Setup(mock => mock.AddExpenseSplitAsync(It.IsAny<ExpenseSplit>()))
                                .ReturnsAsync((ExpenseSplit split) =>
                                {
                                    return split;
                                });

            using (var dbContext = new ApplicationDbContext(_dbContextOptions))
            {
                dbContext.Database.EnsureCreated();

                var expenseBL = new ExpenseBL(_loggerMock.Object, _expenseDalMock.Object,
                                              _groupMemberDalMock.Object, _expenseSplitDalMock.Object,
                                              dbContext);

                // Act
                var createdExpense = await expenseBL.CreateExpenseAsync(expense);

                // Assert
                Assert.NotNull(createdExpense);
                Assert.Equal(expense.ExpenseId, createdExpense.ExpenseId);
                Assert.Equal(expense.GroupId, createdExpense.GroupId);
                Assert.Equal(expense.PaidByUserId, createdExpense.PaidByUserId);
                Assert.Equal(expense.ExpenseAmount, createdExpense.ExpenseAmount);
                Assert.False(createdExpense.IsSettled);
            }
        }

        [Fact]
        public async Task CreateExpenseAsync_InvalidExpense_ThrowsException()
        {
            // Arrange
            var expense = new Expense
            {
                ExpenseId = 1,
                GroupId = 1,
                PaidByUserId = 999,
                ExpenseAmount = 100.0,
                ExpenseCreatedAt = DateTime.UtcNow,
                IsSettled = false
            };

            var groupMembers = new List<GroupMember>
            {
                new GroupMember { UserId = 1, GroupId = 1, GivenAmount = 0, TakenAmount = 0 },
                new GroupMember { UserId = 2, GroupId = 1, GivenAmount = 0, TakenAmount = 0 }
            };

            // Mock IGroupMemberDAL
            _groupMemberDalMock.Setup(mock => mock.GetGroupMembersByGroupIdAsync(1))
                               .ReturnsAsync(groupMembers);

            using (var dbContext = new ApplicationDbContext(_dbContextOptions))
            {
                dbContext.Database.EnsureCreated();

                var expenseBL = new ExpenseBL(_loggerMock.Object, _expenseDalMock.Object,
                                              _groupMemberDalMock.Object, _expenseSplitDalMock.Object,
                                              dbContext);

                // Act and Assert
                await Assert.ThrowsAsync<InvalidOperationException>(() => expenseBL.CreateExpenseAsync(expense));
            }
        }

        [Fact]
        public void UpdateExpenseSettledStatus_ExpenseFound_UpdateSettledStatus()
        {
            // Arrange
            var expenseId = 1;
            var isSettled = true;
            var expense = new Expense
            {
                ExpenseId = expenseId,
                GroupId = 1,
                PaidByUserId = 1,
                ExpenseAmount = 100.0,
                IsSettled = false,
                ExpenseSplit = new List<ExpenseSplit>
                {
                    new ExpenseSplit { SplitWithUserId = 2, SplitAmount = 50.0 },
                    new ExpenseSplit { SplitWithUserId = 3, SplitAmount = 50.0 }
                }
            };

            using (var dbContext = new ApplicationDbContext(_dbContextOptions))
            {
                dbContext.Database.EnsureCreated();
                dbContext.Expenses.Add(expense);
                dbContext.SaveChanges();

                var expenseBL = new ExpenseBL(_loggerMock.Object, _expenseDalMock.Object,
                                              _groupMemberDalMock.Object, _expenseSplitDalMock.Object,
                                              dbContext);

                // Act
                var result = expenseBL.UpdateExpenseSettledStatus(expenseId, isSettled);

                // Assert
                Assert.True(result);

                var updatedExpense = dbContext.Expenses.FirstOrDefault(e => e.ExpenseId == expenseId);
                Assert.NotNull(updatedExpense);
                Assert.True(updatedExpense.IsSettled);
            }
        }

        [Fact]
        public void UpdateExpenseSettledStatus_ExpenseNotFound_ReturnsFalse()
        {
            // Arrange
            var expenseId = 1;
            var isSettled = true;

            using (var dbContext = new ApplicationDbContext(_dbContextOptions))
            {
                dbContext.Database.EnsureCreated();

                var expenseBL = new ExpenseBL(_loggerMock.Object, _expenseDalMock.Object,
                                              _groupMemberDalMock.Object, _expenseSplitDalMock.Object,
                                              dbContext);

                // Act
                var result = expenseBL.UpdateExpenseSettledStatus(expenseId, isSettled);

                // Assert
                Assert.False(result);
            }
        }

        [Fact]
        public void UpdateExpense_ExpenseFound_UpdatesExpense()
        {
            // Arrange
            var expenseId = 1;
            var updatedExpense = new Expense
            {
                ExpenseId = expenseId,
                GroupId = 1,
                PaidByUserId = 1,
                ExpenseAmount = 150.0,
                IsSettled = false,
                ExpenseSplit = new List<ExpenseSplit>
                {
                    new ExpenseSplit { SplitWithUserId = 2, SplitAmount = 75.0 },
                    new ExpenseSplit { SplitWithUserId = 3, SplitAmount = 75.0 }
                }
            };

            var initialExpense = new Expense
            {
                ExpenseId = expenseId,
                GroupId = 1,
                PaidByUserId = 1,
                ExpenseAmount = 100.0,
                IsSettled = false,
                ExpenseSplit = new List<ExpenseSplit>
                {
                    new ExpenseSplit { SplitWithUserId = 2, SplitAmount = 50.0 },
                    new ExpenseSplit { SplitWithUserId = 3, SplitAmount = 50.0 }
                }
            };

            using (var dbContext = new ApplicationDbContext(_dbContextOptions))
            {
                dbContext.Database.EnsureCreated();
                dbContext.Expenses.Add(initialExpense);
                dbContext.SaveChanges();

                var expenseBL = new ExpenseBL(_loggerMock.Object, _expenseDalMock.Object,
                                              _groupMemberDalMock.Object, _expenseSplitDalMock.Object,
                                              dbContext);

                // Act
                expenseBL.UpdateExpense(updatedExpense);

                // Assert
                var updatedExpenseFromDb = dbContext.Expenses.Find(expenseId);
                Assert.NotNull(updatedExpenseFromDb);
                Assert.Equal(updatedExpense.ExpenseAmount, updatedExpenseFromDb.ExpenseAmount);
                Assert.Equal(updatedExpense.ExpenseSplit.Count, updatedExpenseFromDb.ExpenseSplit.Count);
                Assert.Equal(updatedExpense.ExpenseSplit.Sum(es => es.SplitAmount), updatedExpenseFromDb.ExpenseAmount);
            }
        }

        [Fact]
        public void DeleteExpense_ExpenseFound_DeletesExpense()
        {
            // Arrange
            var expenseId = 1;
            var expense = new Expense
            {
                ExpenseId = expenseId,
                GroupId = 1,
                PaidByUserId = 1,
                ExpenseAmount = 100.0,
                IsSettled = false,
                ExpenseSplit = new List<ExpenseSplit>
                {
                    new ExpenseSplit { SplitWithUserId = 2, SplitAmount = 50.0 },
                    new ExpenseSplit { SplitWithUserId = 3, SplitAmount = 50.0 }
                }
            };

            using (var dbContext = new ApplicationDbContext(_dbContextOptions))
            {
                dbContext.Database.EnsureCreated();
                dbContext.Expenses.Add(expense);
                dbContext.SaveChanges();

                var expenseDalMock = new Mock<IExpenseDAL>();
                var groupMemberDalMock = new Mock<IGroupMemberDAL>();
                var expenseSplitDalMock = new Mock<IExpenseSplitDAL>();
                var loggerMock = new Mock<ILogger<ExpenseBL>>();

                var expenseBL = new ExpenseBL(loggerMock.Object, expenseDalMock.Object,
                                              groupMemberDalMock.Object, expenseSplitDalMock.Object,
                                              dbContext);

                // Act
                expenseBL.DeleteExpense(expenseId);

                // Assert
                var deletedExpense = dbContext.Expenses.Find(expenseId);
                Assert.Null(deletedExpense);
            }
        }

        [Fact]
        public void GetExpenseByExpenseId_ExpenseFound_ReturnsExpense()
        {
            // Arrange
            var expenseId = 1;
            var expectedExpense = new Expense
            {
                ExpenseId = expenseId,
                GroupId = 1,
                PaidByUserId = 1,
                ExpenseAmount = 100.0,
                IsSettled = false,
                ExpenseSplit = new List<ExpenseSplit>
                {
                    new ExpenseSplit { SplitWithUserId = 2, SplitAmount = 50.0 },
                    new ExpenseSplit { SplitWithUserId = 3, SplitAmount = 50.0 }
                }
            };

            // Mock dependencies
            var expenseDalMock = new Mock<IExpenseDAL>();
            expenseDalMock.Setup(d => d.GetExpenseByExpenseId(expenseId)).Returns(expectedExpense);

            var groupMemberDalMock = new Mock<IGroupMemberDAL>(); 
            var expenseSplitDalMock = new Mock<IExpenseSplitDAL>(); 
            var dbContextOptionsMock = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDb").Options;
            var dbContextMock = new Mock<ApplicationDbContext>(dbContextOptionsMock);

            var loggerMock = new Mock<ILogger<ExpenseBL>>();

            // Create instance of ExpenseBL
            var expenseBL = new ExpenseBL(loggerMock.Object, expenseDalMock.Object,
                                          groupMemberDalMock.Object, expenseSplitDalMock.Object,
                                          dbContextMock.Object);

            // Act
            var retrievedExpense = expenseBL.GetExpenseByExpenseId(expenseId);

            // Assert
            Assert.NotNull(retrievedExpense);
            Assert.Equal(expectedExpense.ExpenseId, retrievedExpense.ExpenseId);
            Assert.Equal(expectedExpense.GroupId, retrievedExpense.GroupId);
            Assert.Equal(expectedExpense.PaidByUserId, retrievedExpense.PaidByUserId);
            Assert.Equal(expectedExpense.ExpenseAmount, retrievedExpense.ExpenseAmount);
            Assert.Equal(expectedExpense.IsSettled, retrievedExpense.IsSettled);
            Assert.Equal(expectedExpense.ExpenseSplit.Count, retrievedExpense.ExpenseSplit.Count);
            Assert.Equal(expectedExpense.ExpenseSplit.Sum(es => es.SplitAmount), retrievedExpense.ExpenseAmount);
        }

        [Fact]
        public void GetExpenseByExpenseId_ExpenseNotFound_ReturnsNull()
        {
            // Arrange
            var expenseId = 1;

            using (var dbContext = new ApplicationDbContext(_dbContextOptions))
            {
                dbContext.Database.EnsureCreated();

                var expenseBL = new ExpenseBL(_loggerMock.Object, _expenseDalMock.Object,
                                              _groupMemberDalMock.Object, _expenseSplitDalMock.Object,
                                              dbContext);

                // Act
                var retrievedExpense = expenseBL.GetExpenseByExpenseId(expenseId);

                // Assert
                Assert.Null(retrievedExpense);
            }
        }

        [Fact]
        public void GetExpensesByGroupId_GroupHasExpenses_ReturnsExpenses()
        {
            // Arrange
            var groupId = 1;
            var expenses = new List<Expense>
            {
                new Expense
                {
                    ExpenseId = 1,
                    GroupId = groupId,
                    PaidByUserId = 1,
                    ExpenseAmount = 100.0,
                    IsSettled = false,
                    ExpenseSplit = new List<ExpenseSplit>
                    {
                        new ExpenseSplit { SplitWithUserId = 2, SplitAmount = 50.0 },
                        new ExpenseSplit { SplitWithUserId = 3, SplitAmount = 50.0 }
                    }
                },
                new Expense
                {
                    ExpenseId = 2,
                    GroupId = groupId,
                    PaidByUserId = 2,
                    ExpenseAmount = 150.0,
                    IsSettled = false,
                    ExpenseSplit = new List<ExpenseSplit>
                    {
                        new ExpenseSplit { SplitWithUserId = 1, SplitAmount = 75.0 },
                        new ExpenseSplit { SplitWithUserId = 3, SplitAmount = 75.0 }
                    }
                }
            };

            var expenseDalMock = new Mock<IExpenseDAL>();
            expenseDalMock.Setup(d => d.GetExpensesByGroupId(groupId)).Returns(expenses);

            var loggerMock = new Mock<ILogger<ExpenseBL>>();
            var groupMemberDalMock = new Mock<IGroupMemberDAL>();
            var expenseSplitDalMock = new Mock<IExpenseSplitDAL>();
            var dbContextOptionsMock = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName: "TestDb").Options;
            var dbContextMock = new Mock<ApplicationDbContext>(dbContextOptionsMock);

            var expenseBL = new ExpenseBL(loggerMock.Object, expenseDalMock.Object,
                                          groupMemberDalMock.Object, expenseSplitDalMock.Object,
                                          dbContextMock.Object);

            // Act
            var retrievedExpenses = expenseBL.GetExpensesByGroupId(groupId);

            // Assert
            Assert.NotNull(retrievedExpenses);
            Assert.Equal(expenses.Count, retrievedExpenses.Count());

            foreach (var expectedExpense in expenses)
            {
                var retrievedExpense = retrievedExpenses.FirstOrDefault(e => e.ExpenseId == expectedExpense.ExpenseId);
                Assert.NotNull(retrievedExpense);
                Assert.Equal(expectedExpense.GroupId, retrievedExpense.GroupId);
                Assert.Equal(expectedExpense.PaidByUserId, retrievedExpense.PaidByUserId);
                Assert.Equal(expectedExpense.ExpenseAmount, retrievedExpense.ExpenseAmount);
                Assert.Equal(expectedExpense.IsSettled, retrievedExpense.IsSettled);
                Assert.Equal(expectedExpense.ExpenseSplit.Count, retrievedExpense.ExpenseSplit.Count);
                Assert.Equal(expectedExpense.ExpenseSplit.Sum(es => es.SplitAmount), retrievedExpense.ExpenseAmount);
            }
        }

        [Fact]
        public void GetExpensesByGroupId_GroupHasNoExpenses_ReturnsEmpty()
        {
            // Arrange
            var groupId = 1;

            using (var dbContext = new ApplicationDbContext(_dbContextOptions))
            {
                dbContext.Database.EnsureCreated();

                var expenseBL = new ExpenseBL(_loggerMock.Object, _expenseDalMock.Object,
                                              _groupMemberDalMock.Object, _expenseSplitDalMock.Object,
                                              dbContext);

                // Act
                var retrievedExpenses = expenseBL.GetExpensesByGroupId(groupId);

                // Assert
                Assert.NotNull(retrievedExpenses);
                Assert.Empty(retrievedExpenses);
            }
        }

        [Fact]
        public void UpdateExpenseDetails_ExpenseFound_UpdatesExpenseDetails()
        {
            // Arrange
            var expenseId = 1;
            var updatedExpenseDetails = new Expense
            {
                ExpenseId = expenseId,
                GroupId = 1,
                PaidByUserId = 1,
                ExpenseAmount = 150.0,
                IsSettled = false,
                ExpenseSplit = new List<ExpenseSplit>
                {
                    new ExpenseSplit { SplitWithUserId = 2, SplitAmount = 75.0 },
                    new ExpenseSplit { SplitWithUserId = 3, SplitAmount = 75.0 }
                }
            };

            var initialExpense = new Expense
            {
                ExpenseId = expenseId,
                GroupId = 1,
                PaidByUserId = 1,
                ExpenseAmount = 100.0,
                IsSettled = false,
                ExpenseSplit = new List<ExpenseSplit>
                {
                    new ExpenseSplit { SplitWithUserId = 2, SplitAmount = 50.0 },
                    new ExpenseSplit { SplitWithUserId = 3, SplitAmount = 50.0 }
                }
            };

            using (var dbContext = new ApplicationDbContext(_dbContextOptions))
            {
                dbContext.Database.EnsureCreated();
                dbContext.Expenses.Add(initialExpense);
                dbContext.SaveChanges();

                var expenseBL = new ExpenseBL(_loggerMock.Object, _expenseDalMock.Object,
                                              _groupMemberDalMock.Object, _expenseSplitDalMock.Object,
                                              dbContext);

                // Act
                // Act
                expenseBL.UpdateExpenseDetails(updatedExpenseDetails.ExpenseId, updatedExpenseDetails);

                // Assert
                var updatedExpenseFromDb = dbContext.Expenses.Find(expenseId);
                Assert.NotNull(updatedExpenseFromDb);
                Assert.Equal(updatedExpenseDetails.ExpenseAmount, updatedExpenseFromDb.ExpenseAmount);
                Assert.Equal(updatedExpenseDetails.ExpenseSplit.Count, updatedExpenseFromDb.ExpenseSplit.Count);
                Assert.Equal(updatedExpenseDetails.ExpenseSplit.Sum(es => es.SplitAmount), updatedExpenseFromDb.ExpenseAmount);
            }
        }

        [Fact]
        public void UpdateExpenseDetails_ExpenseNotFound_DoesNotUpdate()
        {
            // Arrange
            var expenseId = 1;
            var updatedExpenseDetails = new Expense
            {
                ExpenseId = expenseId,
                GroupId = 1,
                PaidByUserId = 1,
                ExpenseAmount = 150.0,
                IsSettled = false,
                ExpenseSplit = new List<ExpenseSplit>
        {
            new ExpenseSplit { SplitWithUserId = 2, SplitAmount = 75.0 },
            new ExpenseSplit { SplitWithUserId = 3, SplitAmount = 75.0 }
        }
            };

            using (var dbContext = new ApplicationDbContext(_dbContextOptions))
            {
                dbContext.Database.EnsureCreated();

                var expenseBL = new ExpenseBL(_loggerMock.Object, _expenseDalMock.Object,
                                              _groupMemberDalMock.Object, _expenseSplitDalMock.Object,
                                              dbContext);

                // Act
                Assert.Throws<InvalidOperationException>(() => expenseBL.UpdateExpenseDetails(updatedExpenseDetails.ExpenseId, updatedExpenseDetails));

                // Assert
                var updatedExpenseFromDb = dbContext.Expenses.Find(expenseId);
                Assert.Null(updatedExpenseFromDb);
            }
        }
    }
}
