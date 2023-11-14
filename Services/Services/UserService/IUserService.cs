using Services.Services.UserService.Dto;

namespace Services.Services.UserService
{
    public interface IUserService
    {
        Task<UserDto> Register(RegisterDto registerDto);
        Task<UserDto> Login(LoginDto loginDto);
    }
}
