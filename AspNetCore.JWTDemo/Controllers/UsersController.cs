using AspNetCore.JWTDemo.EntityFrameworkCore.Models;
using AspNetCore.JWTDemo.EntityFrameworkCore.Permissions;
using AspNetCore.JWTDemo.Permissions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.JWTDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public UsersController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        [Route("users")]
        [RBACAuthorize(Resource.Users, Operation.WriteOnly)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            return Ok(users);
        }
    }
}
