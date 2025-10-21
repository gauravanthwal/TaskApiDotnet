using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Task.Domain.Dto;
using Task.Domain.Models;
using Task.Infrastructure;

namespace Task.Application.Services
{
    public class AuthService(DatabaseContext context, IConfiguration configuration) : IAuthService
    {
        readonly DatabaseContext _context = context;
        readonly IConfiguration _configuration = configuration;

        public async Task<User?> CreateUserService(CreateUserDto request)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(t=>t.Username == request.Username);

            if (user != null) return null;

            User newUser = new User()
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = "User"
            };

            _context.Users.Add(newUser);

            bool result = await _context.SaveChangesAsync() > 0 ? true : false;

           
            if(!result) return null;
            return newUser;
        }

        public async Task<TokenResponseDto?> LoginUserService(LoginUserDto request)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(t => t.Username == request.Username);
            if (user == null) return null;

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return null;
            }

            TokenResponseDto response = await CreateTokenResponse(user);

            return response;
        }


        private string GenerateAuthToken(User user)
        {
            var keyString = _configuration["Jwt:Key"] ?? throw new Exception("JWT Key is missing!");
            var issuer = _configuration["Jwt:Issuer"] ?? throw new Exception("JWT Issuer is missing!");
            var audience = _configuration["Jwt:Audience"] ?? throw new Exception("JWT Audience is missing!");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // For claim extraction
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("Date", DateTime.Now.ToString("o")),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            var user = await ValidateRefreshTokenAsync(request.UserId, request.RefreshToken);
            if (user == null) return null;
            return await CreateTokenResponse(user);
        }

        private async Task<User?> ValidateRefreshTokenAsync(int userId, string refreshToken)
        {
            var user = await _context.Users.FindAsync(userId);
            if (
                user == null ||
                user.RefreshToekn != refreshToken ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }


        private async Task<string> GenerateAndSaveRefreshToken(User user)
        {
            var refreshToken = GenerateRefreshToken();

            user.RefreshToekn = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            await _context.SaveChangesAsync();
            return refreshToken;
        }


        private string GenerateRefreshToken()
        {
            var randomnumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomnumber);
            return Convert.ToBase64String(randomnumber);
        }

        private async Task<TokenResponseDto> CreateTokenResponse(User user)
        {
            return new TokenResponseDto
            {
                AccessToken = this.GenerateAuthToken(user),
                RefreshToken = await this.GenerateAndSaveRefreshToken(user)
            };
        }
    }
}
