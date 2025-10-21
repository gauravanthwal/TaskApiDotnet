using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task.Application.Services;
using Task.Domain.Dto;

namespace Task.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController: ControllerBase
    {
        IAuthService _authService;
        public AuthController(IAuthService authService) {
            _authService = authService;
        }


        // Create User
        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser(CreateUserDto request)
        {
            var user = await _authService.CreateUserService(request);

            if (user == null) return BadRequest("Couldn't create user");

            return Ok(user);
        }


        // Login User
        [HttpPost("Login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginUserDto request)
        {
            var response = await _authService.LoginUserService(request);

            if (response == null) return BadRequest("Invalid Username or Password");

            return Ok(response);
        }

        // Refresh Token
        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokenAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null) return Unauthorized("Invalid Refresh Token");

            return Ok(result);
        }

    }
}
