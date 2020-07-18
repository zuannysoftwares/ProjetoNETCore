using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
//using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProAgil.api.Dtos;
using ProAgil.Domain.Identity;
using System.Text;
using System.Collections.Generic;
using System.Security.Claims;

namespace ProAgil.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public UserController(IConfiguration config, 
                              UserManager<User> userManager, 
                              SignInManager<User> signInManager, 
                              IMapper mapper)
        {
            _config = config;
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        [HttpGet("GetUser")]
        public async Task<IActionResult> GetUser(UserDto userDto)
        {
            return Ok(userDto);
        }

         [HttpPost("Register")]
         [AllowAnonymous] //Não precisa autenticar para acessar esse método
        public async Task<IActionResult> Register(UserDto userDto)
        {
            try
            {
                var user = _mapper.Map<User>(userDto);
                var result = await _userManager.CreateAsync(user, userDto.Password);

                var userToReturn = _mapper.Map<UserDto>(user);

                if(result.Succeeded)
                {
                    return Created("GetUser", userToReturn);
                }

                return BadRequest(result.Errors);

            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao registrar usuário no banco de dados {ex.Message}");
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userLoginDto.UserName);//Verifico se tem um usuário com o nome digitado na tela
                var result = await _signInManager.CheckPasswordSignInAsync(user, userLoginDto.Password, false);//verifico se o usuário encontrado pelo nome tem a senha passada na tela e não travo o banco(false)

                if(result.Succeeded) // Se tudo estiver certo
                {
                    var appUser = await _userManager.Users //passo as informações desse usuário encontrado para a variavel appUser
                        .FirstOrDefaultAsync(u => u.NormalizedUserName == userLoginDto.UserName.ToString());
                
                    var userToReturn = _mapper.Map<UserLoginDto>(appUser);//Mapeio as informações de appUser para userToReturn

                    return Ok(new {
                        token = GenerateJWToken(appUser).Result, //Gera o TOKEN baseado no usuário encontrado(appUser)
                        user = userToReturn
                    });
                }

                return Unauthorized();// Caso algo dê errado na autorização acima, retorna INAUTORIZADO para acessar o sistema

            }
            catch (System.Exception ex)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao realizar Login. {ex.Message}");
            }
        }

        private async Task<string> GenerateJWToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //var key = new SymmetricSecurityKey(Encoding.ASCII
            //                .GetBytes(_config.GetSection("AppSettings:Token").Value));


            //var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            /*var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds

            };*/

            //var tokenHandler = new JwtSecurityTokenHandler();

            //var token = tokenHandler.CreateToken(tokenDescriptor);

            //return tokenHandler.WriteToken(token);

            return "";
        }
    }
}