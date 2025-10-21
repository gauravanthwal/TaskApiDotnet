using Task.Domain.Dto;
using Task.Domain.Models;

namespace Task.Application.Services
{
    public interface IAuthService
    {
        Task<User?> CreateUserService(CreateUserDto request);
        Task<TokenResponseDto?> LoginUserService(LoginUserDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
