using BusinessLayer;
using BusinessLayer.Interface;
using BusinessObjectLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ExpenseSharing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseSplitController : ControllerBase
    {
        private readonly ILogger<ExpenseSplitController> _logger;
        private readonly IExpenseSplitBL _expenseSplitBl;

        public ExpenseSplitController(ILogger<ExpenseSplitController> logger, IExpenseSplitBL expenseSplitBl)
        {
            _logger = logger;
            _expenseSplitBl = expenseSplitBl;
        }

        [HttpPost("AddExpenseSplit")]
        public async Task<IActionResult> AddExpenseSplit([FromBody] ExpenseSplit expenseSplit)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _expenseSplitBl.AddExpenseSplitAsync(expenseSplit);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new Dictionary<string, string> { { "message", ex.Message } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding expense split.");
                return StatusCode(500, new { message = "An internal error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("GetExpenseSplitsByExpenseId/{expenseId}")]
        public IActionResult GetExpenseSplitsByExpenseId(int expenseId)
        {
            try
            {
                var expenseSplits = _expenseSplitBl.GetExpenseSplitsByExpenseId(expenseId);
                if (expenseSplits == null)
                {
                    return NotFound();
                }
                return Ok(expenseSplits);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching expense splits for expense with id {expenseId}", expenseId);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpPut("UpdateExpenseSplit/{expenseSplitId}")]
        public IActionResult UpdateExpenseSplit(int expenseSplitId, [FromBody] ExpenseSplit expenseSplitModel)
        {
            if (expenseSplitModel == null || expenseSplitId != expenseSplitModel.ExpenseSplitId)
            {
                return BadRequest(new { message = "Expense Split ID mismatch or invalid request body." });
            }

            try
            {
                _expenseSplitBl.UpdateExpenseSplit(expenseSplitModel);
                return Ok(new { message = "Expense Split updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating expense split with ID {expenseSplitId}", expenseSplitId);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpDelete("DeleteExpenseSplit/{expenseSplitId}")]
        public IActionResult DeleteExpenseSplit(int expenseSplitId)
        {
            try
            {
                _expenseSplitBl.DeleteExpenseSplit(expenseSplitId);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Error deleting expense split with id {expenseSplitId}", expenseSplitId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense split with id {expenseSplitId}", expenseSplitId);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }
    }
}
