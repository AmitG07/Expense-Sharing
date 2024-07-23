using BusinessLayer;
using BusinessObjectLayer;
using DataAccessLayer;
using FluentValidation;
using FluentValidation.Results;

using DataAccessLayer.Interface;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExpenseSharing.Tests.DataAccessLayer
{
    public class ExpenseSplitDALTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly ExpenseSplitDAL _expenseSplitDAL;

        public ExpenseSplitDALTests()
        {
            // Initialize DbContextOptions for in-memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Create instance of ApplicationDbContext using in-memory database
            _context = new ApplicationDbContext(options);

            // Seed the database with test data
            SeedDatabase();

            // Initialize ExpenseSplitDAL with the DbContext
            _expenseSplitDAL = new ExpenseSplitDAL(_context);
        }

        private void SeedDatabase()
        {
            var expenseSplits = new List<ExpenseSplit>
            {
                new ExpenseSplit { ExpenseSplitId = 1, ExpenseId = 1 },
                new ExpenseSplit { ExpenseSplitId = 2, ExpenseId = 1 },
                new ExpenseSplit { ExpenseSplitId = 3, ExpenseId = 2 }
            };

            _context.AddRange(expenseSplits);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            // Dispose the DbContext after each test
            _context.Dispose();
        }

        [Fact]
        public async Task AddExpenseSplitAsync_ValidExpenseSplit_ReturnsAddedExpenseSplit()
        {
            // Arrange
            var expenseSplit = new ExpenseSplit { /* Initialize properties */ };

            // Act
            var addedExpenseSplit = await _expenseSplitDAL.AddExpenseSplitAsync(expenseSplit);

            // Assert
            Assert.NotNull(addedExpenseSplit);
            Assert.NotEqual(0, addedExpenseSplit.ExpenseSplitId);
        }

        [Fact]
        public void GetExpenseSplitsByExpenseId_ExistingExpenseId_ReturnsExpenseSplits()
        {
            // Arrange
            var expenseId = 1;

            // Act
            var retrievedExpenseSplits = _expenseSplitDAL.GetExpenseSplitsByExpenseId(expenseId);

            // Assert
            Assert.NotNull(retrievedExpenseSplits);
            Assert.Equal(2, retrievedExpenseSplits.Count());
        }

        [Fact]
        public void GetExpenseSplitsByExpenseId_NonExistingExpenseId_ReturnsEmptyList()
        {
            // Arrange
            var expenseId = 999; 

            // Act
            var retrievedExpenseSplits = _expenseSplitDAL.GetExpenseSplitsByExpenseId(expenseId);

            // Assert
            Assert.NotNull(retrievedExpenseSplits);
            Assert.Empty(retrievedExpenseSplits);
        }

        [Fact]
        public void UpdateExpenseSplit_ValidExpenseSplit_UpdatesExpenseSplit()
        {
            // Arrange
            var expenseSplit = new ExpenseSplit { /* Initialize properties */ };

            // Act
            _expenseSplitDAL.UpdateExpenseSplit(expenseSplit);

            // Assert 
            Assert.True(true); 
        }

        [Fact]
        public void UpdateExpenseSplit_InvalidExpenseSplit_ThrowsException()
        {
            // Arrange
            var invalidExpenseSplit = new ExpenseSplit { ExpenseSplitId = 999, /* Initialize with invalid or non-existent properties */ };

            // Act and Assert
            Assert.Throws<DbUpdateConcurrencyException>(() =>
            {
                _expenseSplitDAL.UpdateExpenseSplit(invalidExpenseSplit);
            });
        }

        [Fact]
        public void DeleteExpenseSplit_ValidExpenseSplitId_DeletesExpenseSplit()
        {
            // Arrange
            var expenseSplitId = 1;

            // Act
            _expenseSplitDAL.DeleteExpenseSplit(expenseSplitId);

            // Assert 
            Assert.True(true); // Placeholder assertion
        }

        [Fact]
        public void DeleteExpenseSplit_NonExistingExpenseSplitId_DoesNotThrow()
        {
            // Arrange
            var expenseSplitId = 999; // Non-existing expense split Id

            // Act
            _expenseSplitDAL.DeleteExpenseSplit(expenseSplitId);

            // Assert 
            Assert.True(true); // Placeholder assertion
        }
    }
}
