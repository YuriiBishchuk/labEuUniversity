using Lab.Business.Models;
using Lab.Business.Services;
using Lab.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lab.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ToDoController : ControllerBase
    {
        private readonly IUserToDoService _userToDoService;
        private readonly IAuthenticatedUserService _authenticatedUserService;

        public ToDoController(IUserToDoService userToDoService, IAuthenticatedUserService authenticatedUserService)
        {
            _userToDoService = userToDoService;
            _authenticatedUserService = authenticatedUserService;
        }

        [HttpGet]
        public async Task<IActionResult> GetToDos()
        {
            try
            {
                int userId = _authenticatedUserService.GetCurrentUserId();
                var toDos = await _userToDoService.GetToDosByUserId(userId);
                return Ok(toDos);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetToDoById(int id)
        {
            try
            {
                int userId = _authenticatedUserService.GetCurrentUserId();
                var toDo = await _userToDoService.GetToDoByIdForUser(userId, id);
                return Ok(toDo);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateToDo([FromBody] ToDo toDo)
        {
            try
            {
                int userId = _authenticatedUserService.GetCurrentUserId();
                await _userToDoService.CreateToDoForUser(userId, toDo);
                return Ok(new { Message = "Завдання створено успішно." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateToDo(int id, [FromBody] ToDo toDo)
        {
            try
            {
                int userId = _authenticatedUserService.GetCurrentUserId();
                await _userToDoService.UpdateToDoForUser(userId, id, toDo);
                return Ok(new { Message = "Завдання оновлено успішно." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDo(int id)
        {
            try
            {
                int userId = _authenticatedUserService.GetCurrentUserId();
                await _userToDoService.DeleteToDoForUser(userId, id);
                return Ok(new { Message = "Завдання видалено успішно." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
