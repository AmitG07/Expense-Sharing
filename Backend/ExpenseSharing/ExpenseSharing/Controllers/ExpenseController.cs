using BusinessLayer.Interface;
using BusinessObjectLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseSharing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly ILogger<ExpenseController> _logger;
        private readonly IExpenseBL _expenseBl;
        private readonly IGroupMemberBL _groupMemberBl;
        private readonly IUserBL _userBl;

        public ExpenseController(ILogger<ExpenseController> logger, IExpenseBL expenseBl, IGroupMemberBL groupMemberBl, IUserBL userBl)
        {
            _logger = logger;
            _expenseBl = expenseBl;
            _groupMemberBl = groupMemberBl;
            _userBl = userBl;
        }

        /*Add Expense*/
        [HttpPost("CreateExpense")]
        public async Task<IActionResult> CreateExpense([FromBody] Expense expense)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _expenseBl.CreateExpenseAsync(expense);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating expense.");
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("GetExpenseByExpenseId/{id}")]
        public IActionResult GetExpenseByExpenseId(int id)
        {
            try
            {
                var expense = _expenseBl.GetExpenseByExpenseId(id);
                if (expense == null)
                {
                    return NotFound();
                }
                return Ok(expense);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching expense with id {id}", id);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("GetExpensesByGroupId/{groupId}")]
        public ActionResult<IEnumerable<Expense>> GetExpensesByGroupId(int groupId)
        {
            try
            {
                var expenses = _expenseBl.GetExpensesByGroupId(groupId);
                if (expenses == null || !expenses.Any())
                {
                    return NotFound();
                }
                return Ok(expenses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching expenses for group with id {groupId}", groupId);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpPut("UpdateExpense/{expenseId}")]
        public IActionResult UpdateExpense(int expenseId, [FromBody] Expense expenseModel)
        {
            if (expenseModel == null || expenseId != expenseModel.ExpenseId)
            {
                return BadRequest(new { message = "Expense ID mismatch or invalid request body." });
            }

            try
            {
                _expenseBl.UpdateExpense(expenseModel);
                return Ok(new { message = "Expense updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating expense with ID {expenseId}", expenseId);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpPut("SettleExpense/{expenseId}")]
        public async Task<IActionResult> SettleExpense(int expenseId)
        {
            try
            {
                var expense = _expenseBl.GetExpenseByExpenseId(expenseId);
                if (expense == null)
                {
                    return NotFound(new { message = "Expense not found." });
                }

                if (expense.IsSettled)
                {
                    return BadRequest(new { message = "Expense is already settled." });
                }

                // Fetch expenses for the group
                var expenses = _expenseBl.GetExpensesByGroupId(expense.GroupId);

                // Check if there are any expenses
                if (expenses.Any())
                {
                    // Mark the expense as settled
                    expense.IsSettled = true;
                    _expenseBl.UpdateExpense(expense);

                    // Update the available balance for each user in the group
                    var groupMembers = await _groupMemberBl.GetGroupMembersByGroupIdAsync(expense.GroupId);
                    foreach (var member in groupMembers)
                    {
                        var user = await _userBl.GetUserByIdAsync(member.UserId);
                        if (user != null)
                        {
                            user.AvailableBalance -= (decimal)member.UserExpense;
                            await _userBl.UpdateUserAsync(user);
                        }
                    }
                }

                return Ok(new { message = "Expense settled successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while settling expense with ID {expenseId}", expenseId);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpDelete("DeleteExpense/{expenseId}")]
        public async Task<IActionResult> DeleteExpense(int expenseId)
        {
            try
            {
                var expense = _expenseBl.GetExpenseByExpenseId(expenseId);
                if (expense == null)
                {
                    return NotFound();
                }

                // Fetch expenses for the group
                var expenses = _expenseBl.GetExpensesByGroupId(expense.GroupId);

                // Check if there are any expenses
                if (expenses.Any())
                {
                    // Update the available balance for each user in the group
                    var groupMembers = await _groupMemberBl.GetGroupMembersByGroupIdAsync(expense.GroupId);
                    foreach (var member in groupMembers)
                    {
                        var user = await _userBl.GetUserByIdAsync(member.UserId);
                        if (user != null)
                        {
                            user.AvailableBalance += (decimal)member.UserExpense;
                            await _userBl.UpdateUserAsync(user);
                        }

                        member.UserExpense = 0;  // Set UserExpense to 0
                        member.GivenAmount = 0;  // Set GivenAmount to 0
                        member.TakenAmount = 0;
                    }
                }

                _expenseBl.DeleteExpense(expenseId);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense with id {expenseId}", expenseId);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }
    }
}
