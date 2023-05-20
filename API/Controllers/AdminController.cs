using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AdminController : ControllerBase
	{
		private readonly UserManager<AppUser> _userManager;
		
		public AdminController(UserManager<AppUser> userManager)
		{
			_userManager = userManager;
		}

		[Authorize(Policy = "RequireAdminRole")]
		[HttpGet("users-with-roles", Name = "UsersWithRoles")]
		public async Task<ActionResult> GetUsersWithRoles()
		{
			var users = await _userManager.Users
				.Include(r => r.UserRoles)
				.ThenInclude(r => r.Role)
				.OrderBy(u => u.UserName)
				.Select(u => new 
				{
					u.Id,
					Username = u.UserName,
					Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
				})
				.ToListAsync();
				
			return Ok(users);
		}
		
		[HttpPatch("edit-roles/{username}")]
		public async Task<ActionResult> EditRole(string username, [FromQuery]string roles)
		{
			var selectedRoles = roles.Split(",").ToArray();	
			var user = await _userManager.FindByNameAsync(username);
			
			if (user == null)
			{
				return NotFound("Could not find user");
			}
			
			var userRoles = await _userManager.GetRolesAsync(user);
			var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
			
			if (!result.Succeeded)
			{
				return BadRequest("Failed to add to roles");
			}		
			
			result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
			
			if (!result.Succeeded)
			{
				return BadRequest("Failed to remove from roles");
			}
			
			return NoContent();
		}
	}
}