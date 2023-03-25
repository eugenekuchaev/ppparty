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
		private readonly IUserRepository _userRepository;
		private readonly IFriendsRepository _friendsRepository;
		
		public FriendsController(IUserRepository userRepository, IFriendsRepository friendsRepository)
		{
			_userRepository = userRepository;
			_friendsRepository = friendsRepository;
		}
		
		[HttpPost("{username}")]
		public async Task<ActionResult> AddFriend(string username)
		{
			var addingToFriendsUserId = User.GetUserId();
			var addedToFriendsUser = await _userRepository.GetUserByUsernameAsync(username);
			var addingToFriendsUser = await _friendsRepository.GetUserWithFriends(addingToFriendsUserId);
			
			if (addedToFriendsUser == null)
			{
				return NotFound();
			}
			
			if (addingToFriendsUser!.UserName == username)
			{
				return BadRequest("You can't add yourself to friends");
			}
			
			var userFriend = await _friendsRepository.GetUserFriend(addingToFriendsUserId, addedToFriendsUser.Id);
			
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
			
			if (await _userRepository.SaveAllAsync()) 
			{
				return Ok();
			}
			
			return BadRequest("Failed to add user to friends");
		}
		
		[HttpDelete("{username}")]
		public async Task<ActionResult> DeleteFriend(string username)
		{
			var addingToFriendsUserId = User.GetUserId();
			var addedToFriendsUser = await _userRepository.GetUserByUsernameAsync(username);
			var addingToFriendsUser = await _friendsRepository.GetUserWithFriends(addingToFriendsUserId);
			
			if (addedToFriendsUser == null)
			{
				return NotFound();
			}
			
			if (addingToFriendsUser!.UserName == username)
			{
				return BadRequest("You can't delete yourself from friends");
			}
			
			var userFriend = await _friendsRepository.GetUserFriend(addingToFriendsUserId, addedToFriendsUser.Id);
			
			if (userFriend == null) 
			{
				return NotFound();
			}
			
			addingToFriendsUser.AddedToFriendsUsers!.Remove(userFriend);
			
			if (await _userRepository.SaveAllAsync()) 
			{
				return Ok();
			}
			
			return BadRequest("Failed to delete user from friends");
		}
		
		[HttpGet]
		public async Task<ActionResult<IEnumerable<FriendDto>>> GetFriends(string predicate)
		{
			if (predicate != "friendrequests" && predicate != "addedtofriends" && predicate != "mutualfriends")
			{
				return BadRequest("This predicate is undefined");
			}
			
			var users = await _friendsRepository.GetFriends(predicate, User.GetUserId());
			
			return Ok(users);
		}
	}
}