using BusinessLayer;
using BusinessLayer.Interface;
using BusinessObjectLayer;
using DataAccessLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseSharing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly ILogger<GroupController> _logger;
        private readonly IGroupBL _groupBl;
        private readonly IGroupMemberBL _groupMemberBl;
        private readonly IUserBL _userBl;
        private readonly IExpenseBL _expenseBl;

        public GroupController(ILogger<GroupController> logger, IGroupBL groupBl, IGroupMemberBL groupMemberBl, IUserBL userBl, IExpenseBL expenseBl)
        {
            _logger = logger;
            _groupBl = groupBl;
            _groupMemberBl = groupMemberBl;
            _userBl = userBl;
            _expenseBl = expenseBl;
        }

        [HttpPost("CreateGroup")]
        public async Task<IActionResult> CreateGroup([FromBody] Group groupModel)
        {
            if (groupModel == null)
            {
                return BadRequest(new { message = "The request body must not be empty.", detail = "groupModel is required." });
            }

            try
            {
                await _groupBl.CreateGroupAsync(groupModel);
                return CreatedAtAction(nameof(GetGroupByGroupId), new { id = groupModel.GroupId }, groupModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while creating a group.");
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("GetAllGroups")]
        public async Task<ActionResult<IEnumerable<Group>>> GetAllGroups()
        {
            try
            {
                var groups = await _groupBl.GetAllGroups();
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all groups.");
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("GetGroupByGroupId/{id}")]
        public async Task<IActionResult> GetGroupByGroupId(int id)
        {
            try
            {
                var group = await _groupBl.GetGroupByGroupIdAsync(id);
                if (group == null)
                {
                    return NotFound();
                }
                return Ok(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching group with id {id}", id);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("GetGroupsByUserId/{userId}")]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroupsByUserId(int userId)
        {
            try
            {
                var groups = await _groupBl.GetGroupsByUserIdAsync(userId);
                if (groups == null || !groups.Any())
                {
                    return NotFound();
                }
                return Ok(groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching groups for user with id {userId}", userId);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpPut("UpdateGroup/{groupId}")]
        public async Task<IActionResult> UpdateGroup(int groupId, [FromBody] Group groupModel)
        {
            if (groupModel == null || groupId != groupModel.GroupId)
            {
                return BadRequest(new { message = "Group ID mismatch or invalid request body." });
            }

            try
            {
                await _groupBl.UpdateGroupAsync(groupModel);
                return Ok(new { message = "Group updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while updating group with ID {groupId}", groupId);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpDelete("DeleteGroup/{groupId}")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            try
            {
                var group = await _groupBl.GetGroupByGroupIdAsync(groupId);
                if (group == null)
                {
                    return NotFound();
                }

                // Fetch expenses for the group
                var expenses = _expenseBl.GetExpensesByGroupId(groupId);

                // Check if there are any expenses
                if (expenses.Any())
                {
                    // Update the available balance for each user in the group
                    var groupMembers = await _groupMemberBl.GetGroupMembersByGroupIdAsync(groupId);
                    foreach (var member in groupMembers)
                    {
                        var user = await _userBl.GetUserByIdAsync(member.UserId);
                        if (user != null)
                        {
                            user.AvailableBalance += (decimal)member.UserExpense;
                            await _userBl.UpdateUserAsync(user);
                        }
                    }
                }

                // Now delete the group
                await _groupBl.DeleteGroupAsync(groupId);
                return Ok(new { message = "Group deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting group with id {groupId}", groupId);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("GetGroupDetails/{groupId}")]
        public async Task<IActionResult> GetGroupDetails(int groupId)
        {
            try
            {
                var group = await _groupBl.GetGroupDetailsAsync(groupId);
                if (group == null)
                {
                    return NotFound();
                }
                return Ok(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching group details for id {groupId}", groupId);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("GetGroupMembersByUserId/{userId}")]
        public async Task<IActionResult> GetGroupMembersByUserId(int userId)
        {
            try
            {
                var members = await _groupMemberBl.GetGroupMembersByUserIdAsync(userId);
                if (members == null || !members.Any())
                {
                    return NotFound();
                }
                return Ok(members);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching group members for user with id {userId}", userId);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }
    }
}
