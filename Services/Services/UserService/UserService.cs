﻿using Core.IdentityEntities;
using Microsoft.AspNetCore.Identity;
using Services.Services.TokenService;
using Services.Services.UserService.Dto;

namespace Services.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signinManager;
        private readonly ITokenService _tokenService;

        public UserService(UserManager<AppUser> userManager,
            SignInManager<AppUser> signinManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signinManager = signinManager;
            _tokenService = tokenService;
        }


        public async Task<UserDto> Login(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null)
                return null;

            var result = await _signinManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return null;

            return new UserDto
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = _tokenService.CreateToken(user)
            };
        }

        public async Task<UserDto> Register(RegisterDto registerDto)
        {
            var user = await _userManager.FindByEmailAsync(registerDto.Email);

            if (user != null)
                return null;

            var appUser = new AppUser
            {
                Email = registerDto.Email,
                DisplayName = registerDto.DisplayName,
                UserName = registerDto.Email.Split('@')[0]
            };

            var result = await _userManager.CreateAsync(appUser, registerDto.Password);

            if (!result.Succeeded)
                return null;

            return new UserDto
            {
                DisplayName = appUser.DisplayName,
                Email = appUser.Email,
                Token = _tokenService.CreateToken(appUser)
            };
        }
    }
}
