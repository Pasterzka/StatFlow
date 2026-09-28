using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.User;
using User_service.Data;
using User_service.Models;

namespace User_service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDbContext _context;

        public UserController(UserDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (request == null) {
                return BadRequest("[ERROR]");
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                email = string.Empty,
                firstName = request.firstName,
                lastName = request.lastName,
                password = string.Empty,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

    }
}
