using DigiDocs.dtos;
using DigiDocs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DigiDocs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public readonly DigidocsContext _context;
        public AuthenticationController(DigidocsContext context)
        {
            _context = context;
        }
        [HttpPost()]
        public IActionResult Login([FromBody] UserRequestDto loginData)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == loginData.Username && u.Password == loginData.Password);
            if (user != null)
            {
                return Ok(new { message = "Login successful", userId = user.Id, role = user.role.ToString(), name = user.name });
            }
            else
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

        }
        [HttpPost()]
        [Route("register")]
        public IActionResult register ([FromBody] RegisterRequestDto AssistantRegister)  
        { 
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == AssistantRegister.Username);
            if (existingUser != null)
            {
                return Conflict(new { message = "Username already exists" });
            }
            User newUser = new User
            {
                Username = AssistantRegister.Username,
                Password = AssistantRegister.Password,
                name = AssistantRegister.Name,
                role = DigiDocs.Enums.Roles.assistant
            };
            _context.Users.Add(newUser);
            _context.SaveChanges();
            return Ok (new { message = "Registration successful", userId = newUser.Id });


        }
    }

}
