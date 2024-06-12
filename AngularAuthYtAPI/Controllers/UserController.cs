using Lab.Business.Models;
using Lab.Business.Services;
using Lab.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Lab.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] UserLoginModel userObj)
        {
            if (userObj == null)
                return BadRequest();

            try
            {
                var token = await _userService.Authenticate(userObj);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> AddUser([FromBody] UserRegistrationModel userObj)
        {
            if (userObj == null)
                return BadRequest();

            try
            {
                await _userService.AddUser(userObj);
                return Ok(new { Message = "Користувача додано!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenApiModel tokenApiDto)
        {
            if (tokenApiDto is null)
                return BadRequest("Неправильний запит клієнта");

            try
            {
                var token = await _userService.Refresh(tokenApiDto);
                return Ok(token);
            }
            catch (Exception ex)
            {       
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}