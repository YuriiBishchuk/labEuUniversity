using Lab.Business.Models;
using Lab.Business.Services;
using Lab.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class ToDoController : ControllerBase
{
    private readonly IUserService _userService;

    public ToDoController(IUserService userService)
    {
        _userService = userService;
    }

}
