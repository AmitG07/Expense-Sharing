using BusinessLayer.Interface;
using BusinessObjectLayer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseSharing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GroupMemberController : ControllerBase
    {
        private readonly IGroupMemberBL _groupMemberBl;

        public GroupMemberController(IGroupMemberBL groupMemberBl)
        {
            _groupMemberBl = groupMemberBl;
        }

        [HttpPost("AddMemberToGroup")]
        public async Task<IActionResult> AddMemberToGroup([FromBody] GroupMember groupMember)
        {
            try
            {
                var result = await _groupMemberBl.AddMemberToGroupAsync(groupMember);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, new { message = "An internal error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("GetGroupMemberById/{id}")]
        public async Task<IActionResult> GetGroupMemberById(int id)
        {
            try
            {
                var groupMember = await _groupMemberBl.GetGroupMemberByIdAsync(id);
                if (groupMember == null)
                {
                    return NotFound();
                }
                return Ok(groupMember);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, new { message = "An internal error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("GetGroupMembersByGroupId/{groupId}")]
        public async Task<IActionResult> GetGroupMembersByGroupId(int groupId)
        {
            try
            {
                var groupMembers = await _groupMemberBl.GetGroupMembersByGroupIdAsync(groupId);

                if (!groupMembers.Any())
                {
                    return NotFound();
                }

                return Ok(groupMembers);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, new { message = "An internal error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("GetGroupMembersByUserId/{userId}")]
        public async Task<IActionResult> GetGroupMembersByUserId(int userId)
        {
            try
            {
                var groupMembers = await _groupMemberBl.GetGroupMembersByUserIdAsync(userId);

                if (groupMembers == null || !groupMembers.Any())
                {
                    return NotFound();
                }

                return Ok(groupMembers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An internal error occurred.", detail = ex.Message });
            }
        }
    }
}