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
		private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
		
		public MessagesController(IMapper mapper, IUnitOfWork unitOfWork)
		{
            _unitOfWork = unitOfWork;
			_mapper = mapper;
		}
		
		[HttpPost]
		public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
		{
			var username = User.GetUsername();
			
			if (username == createMessageDto.RecipientUsername?.ToLower())
			{
				return BadRequest("You cannot send messages to yourself");
			}
			
			var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
			var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername!);
			
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
			
			_unitOfWork.MessageRepository.AddMessage(message);
			
			if (await _unitOfWork.Complete())
			{
				return Ok(_mapper.Map<MessageDto>(message));
			}
			
			return BadRequest("Failed to send message");
		}
		
		[HttpGet("conversations")]
		public async Task<ActionResult<IEnumerable<ConversationDto>>> GetConverstions()
		{
			return Ok(await _unitOfWork.MessageRepository.GetConversations(User.GetUsername()));
		}
		
		[HttpGet("thread/{username}")]
		public async Task<ActionResult<IEnumerable<MessageDto>>> GetMessageThread(string username, 
			[FromQuery]UserParams userParams)
		{
			var currentUsername = User.GetUsername();
			
			var thread = await _unitOfWork.MessageRepository.GetMessageThread(userParams, currentUsername, username);
			
			Response.AddPaginationHeader(thread.CurrentPage, thread.PageSize, thread.TotalCount, thread.TotalPages);
			
			return Ok(thread);
		}
	}
}