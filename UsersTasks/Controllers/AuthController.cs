using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using UsersTasks.Services;
using UsersTasks.Models;

namespace UsersTasks.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService _userService;

        public AuthController(UserService userService)
        {
            _userService = userService;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public ActionResult<User> RegisterUser(User user)
        {
            try
            {
                string token = _userService.RegisterUserAndGetToken(user);
                return Ok(token);
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public ActionResult<User> LoginUser(LoginUser loginUser)
        {
            try
            {
                string token = _userService.LoginUserAndGetToken(loginUser);

                return Ok(token);
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}