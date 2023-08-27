using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using news_api.DTO;
using news_api.Models;
using news_api.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace news_api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public ActionResult<UserModel> Register(User user)
        {
            try
            {
                var userModel = _userService.RegisterUser(user);
                return Ok(new { Status = true, Message = "User registered successfully", Data = userModel });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Status = false, Message = "Registration failed", Error = ex.Message });
            }
        }

        [HttpPost("login")]
        public ActionResult<string> Login(User user)
        {
            try
            {
                var authenticatedUser = _userService.AuthenticateUser(user.Username, user.Password);
                if (authenticatedUser == null)
                {
                    return Unauthorized(new { Status = true, Message = "Username and password is incorrect" });
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("super-super-secret");
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Name, authenticatedUser.Username),
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);
                return Ok(new { Status = true, Message = "User login successfully", Data = tokenString });
            } catch (Exception ex)
            {
                return BadRequest(new { Status = false, Message = "Login failed", Error = ex.Message });
            }
        }
    }
}
