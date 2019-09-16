using System.Threading.Tasks;
using DatingApp.API.Data;
using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Models;
using DatingApp.API.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegiterDto)
        {
            // validar el request
            // if (!ModelState.IsValid)
            //     return BadRequest(ModelState);
            if (!string.IsNullOrEmpty(userForRegiterDto.Username))
                userForRegiterDto.Username = userForRegiterDto.Username.ToLower();

            if (await _repo.UserExists(userForRegiterDto.Username))
                ModelState.AddModelError("Username", "El usuario ya existe"); 

            if (!ModelState.IsValid)     
                return BadRequest(ModelState);        

            var UserToCreate = new User
            {
                Username = userForRegiterDto.Username
            };

            var createdUser = await _repo.Register(UserToCreate, userForRegiterDto.Password);

            return StatusCode(201);

        }

        [HttpPost("login")] /*el login */
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        { 
           // throw new Exception("La computadora dice No!!");
            
             var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null)
                return Unauthorized();

            // generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                    new Claim(ClaimTypes.Name, userFromRepo.Username)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { tokenString });                 
        }
    }
}