using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
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
		private readonly IMapper _mapper;
		private readonly IPhotoService _photoService;
		private readonly LinkTransformer _linkTransformer;
		private readonly InputProcessor _inputProcessor;
        private readonly IUnitOfWork _unitOfWork;

		public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService,
			LinkTransformer linkTransformer, InputProcessor inputProcessor)
		{
            _unitOfWork = unitOfWork;
			_inputProcessor = inputProcessor;
			_linkTransformer = linkTransformer;
			_photoService = photoService;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
			userParams.CurrentUsername = user!.UserName;

			var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

			Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

			return Ok(users);
		}

		[HttpGet("{username}", Name = "GetUser")]
		public async Task<ActionResult<MemberDto?>> GetUser(string username)
		{
			return await _unitOfWork.UserRepository.GetMemberAsync(username);
		}

		[HttpPut("updatename")]
		public async Task<ActionResult> UpdateName(NameUpdateDto nameUpdateDto)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			var trimmedNameUpdateDto = new NameUpdateDto
			{
				FullName = nameUpdateDto.FullName.Trim()
			};

			_mapper.Map(trimmedNameUpdateDto, user);

			_unitOfWork.UserRepository.UpdateUser(user!);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to update user");
		}

		[HttpPut("updatelocation")]
		public async Task<ActionResult> UpdateLocation(LocationUpdateDto locationUpdateDto)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			if (locationUpdateDto.City == "" || locationUpdateDto.City == null)
			{
				locationUpdateDto.City = "Somewhere";
			}

			var trimmedLocationUpdateDto = new LocationUpdateDto
			{
				City = locationUpdateDto.City.Trim(),
				Region = locationUpdateDto.Region?.Trim(),
				Country = locationUpdateDto?.Country?.Trim()
			};

			_mapper.Map(trimmedLocationUpdateDto, user);

			_unitOfWork.UserRepository.UpdateUser(user!);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to update user");
		}

		[HttpPut("updateAbout")]
		public async Task<ActionResult> UpdateAbout(AboutUpdateDto aboutUpdateDto)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			var trimmedAboutUpdateDto = new AboutUpdateDto
			{
				About = aboutUpdateDto?.About?.Trim()
			};

			_mapper.Map(trimmedAboutUpdateDto, user);

			_unitOfWork.UserRepository.UpdateUser(user!);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to update user");
		}

		[HttpPost("addinterests")]
		public async Task<ActionResult> AddInterests([FromBody] string interests)
		{
			if (!_inputProcessor.ValidateInput(interests))
			{
				return BadRequest("You need to add interests");
			}

			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			var userInterests = _inputProcessor.SplitString(interests);
				
			foreach (var userInterest in userInterests)
			{
				if (userInterest.Length > 32)
				{
					return BadRequest("One of the interests is too long");
				}
				
				if (user!.UserInterests!.Any(x => x.InterestName == userInterest))
				{
					return BadRequest("You already have one of these interests");
				}
				
				var userInterestFromDb = await _unitOfWork.UserRepository.GetUserInterestByNameAsync(userInterest);
				
				if (userInterestFromDb != null)
				{
					user!.UserInterests?.Add(userInterestFromDb);
				}
				else
				{
					user!.UserInterests?.Add(new UserInterest { InterestName = userInterest });
				}
			}

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to add interests");
		}

		[HttpDelete("deleteinterest")]
		public async Task<ActionResult> DeleteInterest(string interestName)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			var userInterest = user!.UserInterests?.FirstOrDefault(x => x.InterestName == interestName);

			if (userInterest == null)
			{
				return BadRequest("There's no interest with this name");
			}

			user!.UserInterests?.Remove(userInterest);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to delete interest");
		}

		[HttpPut("updatecontacts")]
		public async Task<ActionResult> UpdateContacts(ContactsUpdateDto contactsUpdateDto)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			_mapper.Map(contactsUpdateDto, user);

			user!.FacebookLink = _linkTransformer.AddHttpsToLink(contactsUpdateDto.FacebookLink);
			user.InstagramLink = _linkTransformer.AddHttpsToLink(contactsUpdateDto.InstagramLink);
			user.TwitterLink = _linkTransformer.AddHttpsToLink(contactsUpdateDto.TwitterLink);
			user.LinkedInLink = _linkTransformer.AddHttpsToLink(contactsUpdateDto.LinkedInLink);
			user.WebsiteLink = _linkTransformer.AddHttpsToLink(contactsUpdateDto.WebsiteLink);

			_unitOfWork.UserRepository.UpdateUser(user);

			if (await _unitOfWork.Complete())
			{
				return NoContent();
			}

			return BadRequest("Failed to update user");
		}

		[HttpPost("add-photo")]
		public async Task<ActionResult<UserPhotoDto>> AddPhoto(IFormFile file)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

			var result = await _photoService.AddPhotoAsync(file, "user");

			if (result.Error != null)
			{
				return BadRequest(result.Error.Message);
			}

			var photo = new UserPhoto
			{
				PhotoUrl = result.SecureUrl.AbsoluteUri,
				PublicId = result.PublicId,
				AppUser = user!
			};

			_unitOfWork.UserRepository.AddPhoto(photo);

			if (await _unitOfWork.Complete())
			{
				return CreatedAtRoute("GetUser", new { Username = user!.UserName }, _mapper.Map<UserPhotoDto>(photo));
			}

			return BadRequest("Problem adding photo");
		}
	}
}