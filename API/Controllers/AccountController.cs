using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AccountController : ControllerBase
	{
		private readonly SignInManager<AppUser> _signInManager;
		private readonly UserManager<AppUser> _userManager;
		private readonly ITokenService _tokenService;
		private readonly IMapper _mapper;

		public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
			ITokenService tokenService, IMapper mapper)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_tokenService = tokenService;
			_mapper = mapper;
		}

		[HttpPost("register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
		{
			if (await UserExists(registerDto.Username))
			{
				return BadRequest("Username is taken");
			}

			var user = _mapper.Map<AppUser>(registerDto);

			user.UserName = registerDto.Username.ToLower();

			user.UserPhoto = new UserPhoto
			{
				PhotoUrl = "https://res.cloudinary.com/duy1fjz1z/image/upload/v1678110186/user_epf5zu.png"
			};

			var result = await _userManager.CreateAsync(user, registerDto.Password);

			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			var roleResult = await _userManager.AddToRoleAsync(user, "Member");

			if (!roleResult.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			var userDto = new UserDto
			{
				Username = user.UserName,
				Token = await _tokenService.CreateToken(user),
				PhotoUrl = user.UserPhoto.PhotoUrl,
				FullName = user.FullName
			};

			var uri = Url.Action("GetUser", "Users", new { username = user.UserName }, Request.Scheme);

			return Created(uri!, userDto);
		}

		[HttpPost("login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
		{
			var user = await _userManager.Users
				.Include(p => p.UserPhoto)
				.SingleOrDefaultAsync(x => x.UserName == loginDto.Username.ToLower());

			if (user == null)
			{
				return Unauthorized();
			}

			var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

			if (!result.Succeeded)
			{
				return Unauthorized();
			}

			return new UserDto
			{
				Username = user.UserName,
				Token = await _tokenService.CreateToken(user),
				PhotoUrl = user.UserPhoto.PhotoUrl,
				FullName = user.FullName
			};
		}

		[Authorize]
		[HttpPatch("update-email")]
		public async Task<ActionResult> UpdateEmail(UpdateEmailDto updateEmailDto)
		{
			var user = await _userManager.FindByNameAsync(User.Identity!.Name);

			if (user == null)
			{
				return NotFound();
			}

			user.Email = updateEmailDto.Email;

			var result = await _userManager.UpdateAsync(user);

			if (!result.Succeeded)
			{
				return BadRequest(result.Errors);
			}

			return NoContent();
		}
		
		[Authorize]
		[HttpPatch("change-password")]
		public async Task<ActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
		{
			if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword) 
			{
				return BadRequest("Passwords don't match");
			}
			
			var user = await _userManager.FindByNameAsync(User.Identity!.Name);

			var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.OldPassword, changePasswordDto.NewPassword);

			if (!result.Succeeded)
			{
				return BadRequest("Wrong password");
			}
			
			return NoContent();
		}

		private async Task<bool> UserExists(string username)
		{
			return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
		}
	}
}