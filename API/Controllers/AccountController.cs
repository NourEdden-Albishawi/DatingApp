
using API.Data;
using API.DTO;
using API.Entities;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
    public class AccountController(DataContext context,ITokenService service) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if(await UserExists(registerDto.Username)) return BadRequest("Username is already in use.");
            return Ok();
            /*using var hmac = new HMACSHA512();
            var user = new User
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return new UserDto { UserName = user.UserName, Token = service.CreateToken(user) };*/
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto dto)
        {
            var user = await context.Users.FirstOrDefaultAsync(user => user.UserName.ToLower() == dto.UserName.ToLower());

            if (user == null) return Unauthorized("Invalid Username.");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            for (int i = 0; i< computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password.");
            }

            return new UserDto { Username = user.UserName, Token = service.CreateToken(user) };
        }
        private async Task<bool> UserExists(string username)
        {
            return await context.Users.AnyAsync(user => user.UserName.ToLower() == username.ToLower());
    }
    }
}
