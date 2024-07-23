using BusinessLayer.Interface;
using BusinessObjectLayer;
using ExpenseSharing.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace ExpenseSharing.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserBL _userBL;
        private readonly ILogger<UserController> _logger;

        public UserController(IUserBL userBL, ILogger<UserController> logger)
        {
            _userBL = userBL;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login(UserLogin login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User user = new User { EmailId = login.Email, Password = login.Password };
                    var validUser = _userBL.Authenticate(user.EmailId, user.Password);
                    if (validUser != null)
                    {
                        return Ok(validUser);
                    }
                    else
                    {
                        return BadRequest(new { message = "Invalid email or password." });
                    }
                }
                return BadRequest(new { message = "Invalid model state." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred during login.");
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("GetUserById/{id}")]
        public IActionResult GetUserById(int id)
        {
            try
            {
                var user = _userBL.GetUserById(id);
                if (user == null)
                {
                    return NotFound();
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user with id {id}", id);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers()
        {
            try
            {
                var users = _userBL.GetAllUsers();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all users.");
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }


        /*// PUT api/User/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, User user)
        {
            try
            {
                if (id != user.UserId)
                {
                    return BadRequest();
                }

                _userBL.UpdateUser(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with id {id}", id);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }

        // DELETE api/User/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var user = _userBL.GetUserById(id);
                if (user == null)
                {
                    return NotFound();
                }

                _userBL.DeleteUser(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with id {id}", id);
                return StatusCode(500, new { message = "An unexpected error occurred.", detail = ex.Message });
            }
        }*/
    }
}
