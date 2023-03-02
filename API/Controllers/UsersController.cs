using System.Security.Claims;
using System.Text.RegularExpressions;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	[Authorize]
	public class UsersController : ControllerBase
	{
		private readonly IUserRepository _userRepository;
		private readonly IMapper _mapper;

		public UsersController(IUserRepository userRepository, IMapper mapper)
		{
			_mapper = mapper;
			_userRepository = userRepository;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
		{
			var users = await _userRepository.GetMembersAsync();

			return Ok(users);
		}

		[HttpGet("{username}")]
		public async Task<ActionResult<MemberDto?>> GetUser(string username)
		{
			return await _userRepository.GetMemberAsync(username);
		}

		[HttpPut("updatename")]
		public async Task<ActionResult> UpdateName(NameUpdateDto nameUpdateDto)
		{
			var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var user = await _userRepository.GetUserByUsernameAsync(username!);
			
			var trimmedNameUpdateDto = new NameUpdateDto
			{
				FullName = nameUpdateDto.FullName.Trim()
			};

			_mapper.Map(trimmedNameUpdateDto, user);

			_userRepository.UpdateUser(user!);

			if (await _userRepository.SaveAllAsync())
			{
				return NoContent();
			}

			return BadRequest("Failed to update user.");
		}

		[HttpPut("updatelocation")]
		public async Task<ActionResult> UpdateLocation(LocationUpdateDto locationUpdateDto)
		{
			var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var user = await _userRepository.GetUserByUsernameAsync(username!);
			
			var trimmedLocationUpdateDto = new LocationUpdateDto 
			{
				City = locationUpdateDto?.City?.Trim(),
				Region = locationUpdateDto?.Region?.Trim(),
				Country = locationUpdateDto?.Country?.Trim()
			};

			_mapper.Map(trimmedLocationUpdateDto, user);

			_userRepository.UpdateUser(user!);

			if (await _userRepository.SaveAllAsync())
			{
				return NoContent();
			}

			return BadRequest("Failed to update user.");
		}
		
		[HttpPut("updateAbout")]
		public async Task<ActionResult> UpdateAbout(AboutUpdateDto aboutUpdateDto)
		{
			var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var user = await _userRepository.GetUserByUsernameAsync(username!);
			
			var trimmedAboutUpdateDto = new AboutUpdateDto 
			{
				About = aboutUpdateDto?.About?.Trim()
			};
			
			_mapper.Map(trimmedAboutUpdateDto, user);

			_userRepository.UpdateUser(user!);
			
			if (await _userRepository.SaveAllAsync())
			{
				return NoContent();
			}

			return BadRequest("Failed to update user.");
		}

		[HttpPost("addinterests")]
		public async Task<ActionResult> AddInterests([FromBody]string interests)
		{
			string pattern = @"^[ ,]*$";

			bool containsOnlySpacesOrCommas = Regex.IsMatch(interests, pattern);

			if (interests == null || interests == "" || containsOnlySpacesOrCommas)
			{
				return BadRequest("You need to add interests.");
			}

			var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var user = await _userRepository.GetUserByUsernameAsync(username!);

			List<string> userInterests = interests
				.Split(',', StringSplitOptions.RemoveEmptyEntries)
				.Select(s => s.Trim())
				.ToList();

			foreach (var userInterest in userInterests)
			{
				if (userInterest != "") 
				{
					var userInterestFromDb = await _userRepository.GetUserInterestByNameAsync(userInterest);
					
					if (userInterestFromDb != null)
					{
						user!.UserInterests?.Add(userInterestFromDb);
					}
					else 
					{
						user!.UserInterests?.Add(new UserInterest { InterestName = userInterest });
					}
				}
			}

			if (await _userRepository.SaveAllAsync())
			{
				return NoContent();
			}

			return BadRequest("Failed to add interests.");
		}

		[HttpDelete("deleteinterest")]
		public async Task<ActionResult> DeleteInterest(string interestName)
		{
			var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var user = await _userRepository.GetUserByUsernameAsync(username!);

			var userInterest = user!.UserInterests?.FirstOrDefault(x => x.InterestName == interestName);

			if (userInterest == null)
			{
				return BadRequest("There's no interest with this name.");
			}

			user!.UserInterests?.Remove(userInterest);

			if (await _userRepository.SaveAllAsync())
			{
				return NoContent();
			}

			return BadRequest("Failed to delete interest.");
		}

		[HttpPut("updatecontacts")]
		public async Task<ActionResult> UpdateContacts(ContactsUpdateDto contactsUpdateDto)
		{
			var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

			var user = await _userRepository.GetUserByUsernameAsync(username!);

			_mapper.Map(contactsUpdateDto, user);

			_userRepository.UpdateUser(user!);

			if (await _userRepository.SaveAllAsync())
			{
				return NoContent();
			}

			return BadRequest("Failed to update user");
		}
	}
}