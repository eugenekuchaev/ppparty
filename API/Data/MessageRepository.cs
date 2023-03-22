using API.DTOs;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
	public class MessageRepository : IMessageRepository
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;

		public MessageRepository(DataContext context, IMapper mapper)
		{
			_mapper = mapper;
			_context = context;
		}

		public void AddMessage(Message message)
		{
			_context.Messages.Add(message);
		}

		public void DeleteMessage(Message message)
		{
			_context.Messages.Remove(message);
		}

		public async Task<Message?> GetMessage(int id)
		{
			return await _context.Messages.FindAsync(id);
		}

		public async Task<PagedList<MessageDto>> GetMessageThread(UserParams userParams, 
			string currentUsername, string recipientUsername)
		{
			var messages = await _context.Messages
				.Include(u => u.Sender).ThenInclude(p => p.UserPhoto)
				.Include(u => u.Recipient).ThenInclude(p => p.UserPhoto)
				.Where(m => m.Recipient.UserName == currentUsername &&
					m.Sender.UserName == recipientUsername ||
					m.Recipient.UserName == recipientUsername &&
					m.Sender.UserName == currentUsername
				)
				.OrderByDescending(m => m.MessageSent)
				.ToListAsync();

			var unreadMessages = messages
				.Where(m => m.DateRead == null && m.Recipient.UserName == currentUsername)
				.ToList();

			if (unreadMessages.Any())
			{
				foreach (var message in unreadMessages)
				{
					message.DateRead = DateTime.Now;
				}

				await _context.SaveChangesAsync();
			}

			var mappedMessages = _mapper.Map<IEnumerable<MessageDto>>(messages).AsQueryable();
			
			return PagedList<MessageDto>.Create(mappedMessages, userParams.PageNumber, userParams.PageSize);
		}

		public async Task<bool> SaveAllAsync()
		{
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<IEnumerable<ConversationDto>> GetConversations(string currentUsername)
		{
			return await _context.Messages
				.Where(m => m.RecipientUsername == currentUsername || m.SenderUsername == currentUsername)
				.GroupBy(m => m.RecipientUsername == currentUsername ? m.SenderUsername : m.RecipientUsername)
				.Select(g => new ConversationDto
				{
					FriendFullName = g
						.OrderByDescending(m => m.MessageSent)
						.Select(m => m.Sender.UserName == currentUsername ? m.Recipient.FullName : m.Sender.FullName)
						.FirstOrDefault()!,
					FriendUsername = g
						.OrderByDescending(m => m.MessageSent)
						.Select(m => m.Sender.UserName == currentUsername ? m.Recipient.UserName : m.Sender.UserName)
						.FirstOrDefault()!,
					FriendPhotoUrl = g
						.OrderByDescending(m => m.MessageSent)
						.Select(m => m.Sender.UserName == currentUsername ? m.Recipient.UserPhoto.PhotoUrl : m.Sender.UserPhoto.PhotoUrl)
						.FirstOrDefault()!,
					LastMessageAuthorName = g
						.OrderByDescending(m => m.MessageSent)
						.Select(m => m.Sender.UserName == currentUsername ? "You" : m.Sender.FullName)
						.FirstOrDefault()!,
					LastMessageContent = g
						.OrderByDescending(m => m.MessageSent)
						.Select(m => m.Content)
						.FirstOrDefault()!,
					LastMessageSent = g
						.OrderByDescending(m => m.MessageSent)
						.Select(m => m.MessageSent)
						.FirstOrDefault(),
					LastMessageRead = g
						.OrderByDescending(m => m.MessageSent)
						.Select(m => m.DateRead)
						.FirstOrDefault()
				})
				.OrderByDescending(c => c.LastMessageSent)
				.ToListAsync();
		}
	}
}