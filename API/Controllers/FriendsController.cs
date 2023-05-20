using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class FriendsController : ControllerBase
	{
		private readonly IUnitOfWork _unitOfWork;
		
		public FriendsController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		
		[HttpPatch("add-friend/{username}")]
		public async Task<ActionResult> AddFriend(string username)
		{
			var addingToFriendsUserId = User.GetUserId();
			var addingToFriendsUser = await _unitOfWork.FriendsRepository.GetUserWithFriends(addingToFriendsUserId);
			var addedToFriendsUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
			
			if (addedToFriendsUser == null)
			{
				return NotFound();
			}
			
			if (addingToFriendsUser!.UserName == username)
			{
				return BadRequest("You can't add yourself to friends");
			}
			
			var userFriend = await _unitOfWork.FriendsRepository.GetUserFriend(addingToFriendsUserId, addedToFriendsUser.Id);
			
			if (userFriend != null) 
			{
				return BadRequest("You've already added this user to friends");
			}
			
			userFriend = new AppUserFriend
			{
				AddingToFriendsUserId = addingToFriendsUserId,
				AddedToFriendsUserId = addedToFriendsUser.Id
			};
			
			addingToFriendsUser.AddedToFriendsUsers!.Add(userFriend);
			
			if (await _unitOfWork.Complete()) 
			{
				return NoContent();
			}
			
			return BadRequest("Failed to add user to friends");
		}
		
		[HttpPatch("delete-friend/{username}")]
		public async Task<ActionResult> DeleteFriend(string username)
		{
			var addingToFriendsUserId = User.GetUserId();
			var addingToFriendsUser = await _unitOfWork.FriendsRepository.GetUserWithFriends(addingToFriendsUserId);
			var addedToFriendsUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
			
			if (addedToFriendsUser == null)
			{
				return NotFound();
			}
			
			if (addingToFriendsUser!.UserName == username)
			{
				return BadRequest("You can't delete yourself from friends");
			}
			
			var userFriend = await _unitOfWork.FriendsRepository.GetUserFriend(addingToFriendsUserId, addedToFriendsUser.Id);
			
			addingToFriendsUser.AddedToFriendsUsers!.Remove(userFriend!);
			
			if (await _unitOfWork.Complete()) 
			{
				return NoContent();
			}
			
			return BadRequest("Failed to delete user from friends");
		}
		
		[HttpGet]
		public async Task<ActionResult<IEnumerable<FriendDto>>> GetFriends(string predicate)
		{
			if (predicate != "friend-requests" && predicate != "added-to-friends" && predicate != "mutual-friends")
			{
				return BadRequest("This predicate is undefined");
			}
			
			var users = await _unitOfWork.FriendsRepository.GetFriends(predicate, User.GetUserId());
			
			return Ok(users);
		}
	}
}