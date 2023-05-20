using API.DTOs;
using API.Extensions;
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
		
		[HttpGet("conversations", Name = "GetConversations")]
		public async Task<ActionResult<IEnumerable<ConversationDto>>> GetConverstions()
		{
			return Ok(await _unitOfWork.MessageRepository.GetConversations(User.GetUsername()));
		}
	}
}