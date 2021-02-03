using ASPNET_WebAPI.Data;
using ASPNET_WebAPI.Models;
using AuthenticationPlugin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASPNET_WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private CinemaDbContext _dbContext;
        private IConfiguration _configuration;
        private readonly AuthService _auth;

        public UsersController(CinemaDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;

            _configuration = configuration;
            _auth = new AuthService(_configuration);
        }

        [HttpPost]
        public IActionResult Register([FromForm] User userObj)
        {
            // Where function -> Lambda function taking in user each time and performs comparison
            // chained with SingleOrDefault() to return the first instance of a match.
            var userWithSameEmail = _dbContext.Users.Where(u => u.Email == userObj.Email).SingleOrDefault();

            if (userWithSameEmail != null)
            {
                return BadRequest("Email is already registered.");
            }

            var newUser = new User
            {
                Name = userObj.Name,
                Email = userObj.Email,
                Password = SecurePasswordHasherHelper.Hash(userObj.Password),
                Role = "User"
            };

            _dbContext.Users.Add(newUser);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status200OK);
        }

        [HttpPost]
        public IActionResult Login([FromForm] User userObj)
        {
            var foundUser = _dbContext.Users.FirstOrDefault(u => u.Email == userObj.Email);
            if (foundUser == null)
            {
                return NotFound();
            }

            if (!SecurePasswordHasherHelper.Verify(userObj.Password, foundUser.Password))
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, userObj.Email),
                new Claim(ClaimTypes.Email,userObj.Email),
                new Claim(ClaimTypes.Role, foundUser.Role)
            };

            var token = _auth.GenerateAccessToken(claims);

            return new ObjectResult(new
            {
                access_token = token.AccessToken,
                expires_in = token.ExpiresIn,
                token_type = token.TokenType,
                creation_time = token.ValidFrom,
                expiration_time = token.ValidTo,
                user_id = foundUser.Id
            });
        }
    }
}