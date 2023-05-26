using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
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

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername,
            string recipientUsername)
        {
            var messages = await _context.Messages
                .Include(u => u.Sender).ThenInclude(p => p.UserPhoto)
                .Include(u => u.Recipient).ThenInclude(p => p.UserPhoto)
                .Where(m => m.Recipient.UserName == currentUsername && m.RecipientDeleted == false
                        && m.Sender.UserName == recipientUsername
                        || m.Recipient.UserName == recipientUsername
                        && m.Sender.UserName == currentUsername && m.SenderDeleted == false
                )
                .OrderByDescending(m => m.MessageSent)
                .ToListAsync();

            var unreadMessages = messages.Where(m => m.DateRead == null
                && m.Recipient.UserName == currentUsername).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                }
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<IEnumerable<ConversationDto>> GetConversations(string currentUsername)
        {
            var conversations = await _context.Messages
                .Include(m => m.Sender).ThenInclude(n => n.UserPhoto)
                .Include(m => m.Recipient).ThenInclude(n => n.UserPhoto)
                .Where(m => m.RecipientUsername == currentUsername || m.SenderUsername == currentUsername)
                .GroupBy(m => m.SenderUsername == currentUsername ? m.RecipientUsername : m.SenderUsername)
                .Select(g => g.OrderByDescending(m => m.MessageSent).FirstOrDefault()).ToListAsync();
            var conversationDtos = conversations.Select(m => new ConversationDto
            {
                FriendFullName = m!.SenderUsername == currentUsername ? m.Recipient.FullName : m.Sender.FullName,
                FriendUsername = m.SenderUsername == currentUsername ? m.RecipientUsername : m.SenderUsername,
                FriendPhotoUrl = m.SenderUsername == currentUsername ? m.Recipient.UserPhoto.PhotoUrl : m.Sender.UserPhoto.PhotoUrl,
                LastMessageAuthorName = m.SenderUsername == currentUsername ? "You" : m.Sender.FullName,
                LastMessageContent = m.Content,
                LastMessageSent = m.MessageSent,
                LastMessageRead = m.DateRead
            }).OrderByDescending(c => c.LastMessageSent).ToList();
            return conversationDtos;
        }

        public void AddGroup(Group group)
        {
            _context.Groups.Add(group);
        }

        public void RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<Connection?> GetConnection(string connectionId)
        {
            return await _context.Connections.FindAsync(connectionId);
        }

        public async Task<Group?> GetMessageGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.GroupName == groupName);
        }

        public async Task<Group?> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(c => c.Connections)
                .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }
    }
}