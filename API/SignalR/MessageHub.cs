using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace API.SignalR
{
	public class MessageHub : Hub
	{
		private readonly IMessageRepository _messageRepository;
		private readonly IMapper _mapper;
		private readonly IUserRepository _userRepository;
		private readonly IHubContext<PresenceHub> _presenceHub;
		private readonly PresenceTracker _tracker;
		private readonly DataContext _context;

		public MessageHub(IMessageRepository messageRepository, IMapper mapper, IUserRepository userRepository,
			IHubContext<PresenceHub> presenceHub, PresenceTracker tracker, DataContext context)
		{
			_context = context;
			_tracker = tracker;
			_presenceHub = presenceHub;
			_userRepository = userRepository;
			_mapper = mapper;
			_messageRepository = messageRepository;
		}

		public override async Task OnConnectedAsync()
		{
			var httpContext = Context.GetHttpContext();
			var otherUser = httpContext?.Request.Query["user"].ToString();

			var groupName = GetGroupName(Context.User!.GetUsername(), otherUser!);
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
			var group = await AddToGroup(groupName);
			await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

			var messages = await _messageRepository.GetMessageThreadWithoutParams(Context.User!.GetUsername(), otherUser!);

			await Clients.Caller.SendAsync("ReceiveMessageThread", messages);

			SendUpdateConversations(otherUser!);
		}

		public override async Task OnDisconnectedAsync(Exception? exception)
		{
			var group = await RemoveFromMessageGroup();
			await Clients.Group(group.GroupName).SendAsync("UpdatedGroup", group);
			await base.OnDisconnectedAsync(exception);
		}

		public async Task SendMessage(CreateMessageDto createMessageDto)
		{

			var username = Context.User!.GetUsername();

			if (username == createMessageDto.RecipientUsername?.ToLower())
			{
				throw new HubException("You cannot send messages to yourself");
			}

			var sender = await _userRepository.GetUserByUsernameAsync(username);
			var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername!);

			var senderFriendship = await _context.Friends
				.FirstOrDefaultAsync(f => f.AddingToFriendsUserId == sender!.Id && f.AddedToFriendsUserId == recipient!.Id);

			var recipientFriendship = await _context.Friends
				.FirstOrDefaultAsync(f => f.AddingToFriendsUserId == recipient!.Id && f.AddedToFriendsUserId == sender!.Id);

			if (senderFriendship == null || recipientFriendship == null)
			{
				throw new HubException("You are not friends with this user");
			}

			if (recipient == null)
			{
				throw new HubException("Not found user");
			}

			var message = new Message
			{
				Sender = sender!,
				Recipient = recipient,
				SenderUsername = sender!.UserName,
				RecipientUsername = recipient.UserName,
				Content = createMessageDto.Content
			};

			var groupName = GetGroupName(sender.UserName, recipient.UserName);

			var group = await _messageRepository.GetMessageGroup(groupName);

			if (group!.Connections.Any(x => x.Username == recipient.UserName))
			{
				message.DateRead = DateTime.UtcNow;
			}
			else
			{
				var connections = await _tracker.GetConnectionsForUser(recipient.UserName);

				if (connections != null)
				{
					await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
						new { username = sender.UserName, fullName = sender.FullName });
				}
			}

			_messageRepository.AddMessage(message);

			if (await _messageRepository.SaveAllAsync())
			{
				await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));

				SendUpdateConversations(recipient.UserName);
			}
		}

		private async Task<Group> AddToGroup(string groupName)
		{
			var group = await _messageRepository.GetMessageGroup(groupName);
			var connection = new Connection(Context.ConnectionId, Context.User!.GetUsername());

			if (group == null)
			{
				group = new Group(groupName);
				_messageRepository.AddGroup(group);
			}

			group.Connections.Add(connection);

			if (await _messageRepository.SaveAllAsync())
			{
				return group;
			}

			throw new HubException("Failed to join group");
		}

		private async Task<Group> RemoveFromMessageGroup()
		{
			var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
			var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
			_messageRepository.RemoveConnection(connection!);

			if (await _messageRepository.SaveAllAsync())
			{
				return group!;
			}

			throw new HubException("Failed to remove from group");
		}

		private string GetGroupName(string caller, string other)
		{
			var stringCompare = string.CompareOrdinal(caller, other) < 0;
			return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
		}

		private async void SendUpdateConversations(string otherUsername)
		{
			var connections = await _tracker.GetConnectionsForUser(otherUsername);
			
			if (connections == null)
			{
				return;
			}
			
			await _presenceHub.Clients.Clients(connections).SendAsync("UpdateConversations", new {});
		}
	}
}