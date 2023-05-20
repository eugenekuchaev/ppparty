using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
	public class MessageHub : Hub
	{
		private readonly IMapper _mapper;
		private readonly IHubContext<PresenceHub> _presenceHub;
		private readonly PresenceTracker _tracker;
		private readonly DataContext _context;
		private readonly IUnitOfWork _unitOfWork;

		public MessageHub(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<PresenceHub> presenceHub, 
			PresenceTracker tracker, DataContext context)
		{
			_unitOfWork = unitOfWork;
			_context = context;
			_tracker = tracker;
			_presenceHub = presenceHub;
			_mapper = mapper;
		}

		public override async Task OnConnectedAsync()
		{
			var httpContext = Context.GetHttpContext();
			var otherUser = httpContext?.Request.Query["user"].ToString();

			var groupName = GetGroupName(Context.User!.GetUsername(), otherUser!);
			await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
			var group = await AddToGroup(groupName);
			await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

			var messages = await _unitOfWork.MessageRepository
				.GetMessageThread(Context.User!.GetUsername(), otherUser!);
				
			if (_unitOfWork.HasChanges()) 
			{
				await _unitOfWork.Complete();
			}

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
			if (createMessageDto.Content.Length > 1000)
			{
				throw new HubException("The message is too long");
			}

			var username = Context.User!.GetUsername();

			if (username == createMessageDto.RecipientUsername?.ToLower())
			{
				throw new HubException("You cannot send messages to yourself");
			}

			var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
			var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername!);
			
			if (recipient == null)
			{
				throw new HubException("Not found user");
			}

			if (!await _unitOfWork.FriendsRepository.CheckIfUsersAreFriends(sender!.Id, recipient.Id))
			{
				throw new HubException("You are not friends with this user");
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

			var group = await _unitOfWork.MessageRepository
				.GetMessageGroup(groupName);

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

			_unitOfWork.MessageRepository
				.AddMessage(message);

			if (await _unitOfWork.Complete())
			{
				await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));

				SendUpdateConversations(recipient.UserName);
			}
		}

		private async Task<Group> AddToGroup(string groupName)
		{
			var group = await _unitOfWork.MessageRepository
				.GetMessageGroup(groupName);
			var connection = new Connection(Context.ConnectionId, Context.User!.GetUsername());

			if (group == null)
			{
				group = new Group(groupName);
				_unitOfWork.MessageRepository
					.AddGroup(group);
			}

			group.Connections.Add(connection);

			if (await _unitOfWork.Complete())
			{
				return group;
			}

			throw new HubException("Failed to join group");
		}

		private async Task<Group> RemoveFromMessageGroup()
		{
			var group = await _unitOfWork.MessageRepository.GetGroupForConnection(Context.ConnectionId);
			var connection = group?.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
			_unitOfWork.MessageRepository.RemoveConnection(connection!);

			if (await _unitOfWork.Complete())
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