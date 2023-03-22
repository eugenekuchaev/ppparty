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
	public class MessagesController : ControllerBase
	{
		private readonly IUserRepository _userRepository;
		private readonly IMessageRepository _messageRepository;
		private readonly IMapper _mapper;
		
		public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
		{
			_mapper = mapper;
			_userRepository = userRepository;
			_messageRepository = messageRepository;
		}
		
		[HttpPost]
		public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
		{
			var username = User.GetUsername();
			
			if (username == createMessageDto.RecipientUsername?.ToLower())
			{
				return BadRequest("You cannot send messages to yourself");
			}
			
			var sender = await _userRepository.GetUserByUsernameAsync(username);
			var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername!);
			
			if (recipient == null) 
			{
				return NotFound();
			}
			
			var message = new Message
			{
				Sender = sender!,
				Recipient = recipient,
				SenderUsername = sender!.UserName,
				RecipientUsername = recipient.UserName,
				Content = createMessageDto.Content
			};
			
			_messageRepository.AddMessage(message);
			
			if (await _messageRepository.SaveAllAsync())
			{
				return Ok(_mapper.Map<MessageDto>(message));
			}
			
			return BadRequest("Failed to send message");
		}
		
		[HttpGet("conversations")]
		public async Task<ActionResult<IEnumerable<ConversationDto>>> GetConverstions()
		{
			return Ok(await _messageRepository.GetConversations(User.GetUsername()));
		}
		
		[HttpGet("thread/{username}")]
		public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username, 
			[FromQuery]UserParams userParams)
		{
			var currentUsername = User.GetUsername();
			
			var thread = await _messageRepository.GetMessageThread(userParams, currentUsername, username);
			
			Response.AddPaginationHeader(thread.CurrentPage, thread.PageSize, thread.TotalCount, thread.TotalPages);
			
			return Ok(thread);
		}
	}
}