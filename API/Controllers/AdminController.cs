using API.Entities;
using API.Extensions;
using API.Interfaces;
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
		private readonly IUnitOfWork _unitOfWork;
		
		public AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
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
		
		// Event deletion is available only for moderators or admins in cases when 
		// an event contains inappropriate or illegal information. Otherwise, it 
		// should be cancelled by the user so all the participants are notified.
		[Authorize(Policy = "RequireModeratorRole")]
		[HttpDelete("delete-event/{eventId}")]
		public async Task<ActionResult> DeleteEvent(int eventId)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
			var appEvent = await _unitOfWork.EventRepository.GetEventEntityAsync(eventId);

			if (appEvent == null)
			{
				return NotFound("There's no event with this Id");
			}

			_unitOfWork.EventRepository.DeleteEvent(appEvent);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to delete event");
		}
	}
}