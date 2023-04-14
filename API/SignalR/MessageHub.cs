using API.DTO;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{   
    [Authorize]
    public class MessageHub : Hub
    {
        // return message thread between them and the user they're connected to (instead of refreshing pages everytime toreceive update on message)
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;

        //inject presencehub inside messagehub which allow online users to receive notification from presencehub (messages sent but recipient not in message tab)
        public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, 
        IMapper mapper, IHubContext<PresenceHub> presenceHub)
        {
            _presenceHub = presenceHub;
            _mapper = mapper;
            _userRepository = userRepository;
            _messageRepository = messageRepository;

        }

        public override async Task OnConnectedAsync()
        {
            // Identify username of who is connected to the hub (the username who click on user's messages tab)
            //access http context as when user makes a connection to singalR hub, they send up htt request to initialize that connection,
            //this gives opportunity to send something in a query string.(get information).
            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext.Request.Query["user"]; // query for user's username

            //put users in a group (combination of one username and other username)    
            var groupName = GetGroupName(Context.User.GetUsername(), otherUser);
            // adds the group to signalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            //add group to database
            var group = await AddToGroup(groupName);
            
            //Updating the group so that client knows who's inside a group at any point of time.
            //So that Client that receive this can use this method to update list of members in a particular message group
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

            var messages = await _messageRepository
                .GetMessageThread(Context.User.GetUsername(), otherUser) ;
            
            //specify groups name that you want to send it to.
            //receive messages from signalR message hub instead of making API call to go and get messages.
            //definitely need the message thread as they connect to the message hub.
            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }

        //moves to different part of app in this hub as it will not always be connecteed like presencehub
        //they are automatically removed by signalR from any groups they belong to.
        public override async Task OnDisconnectedAsync(Exception exception)
        {  
            var group = await RemoveFromMessageGroup();
            // has to be the same as method above as clients just want to know who's in a group at any given time.
            //Only send message to users in group as signalR do not waste time or resource to send message to an empty group.
            await Clients.Group(group.Name).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync(exception);
        }

        //send message via singalR hub and enable live communication where messages received live by other side of conversation as long as they're connected to the same hub.
        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var username = Context.User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                throw new HubException("You cannot send messages to yourself");

            var sender = await _userRepository.GetUserByUsernameAsync(username);
            var recipient = await _userRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);            
        
            if (recipient == null) throw new HubException("Not found user");

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var group = await _messageRepository.GetMessageGroup(groupName);

            //check for same message hub connections to see if username inside matches the recipient username and mark message as read. 
            if (group.Connections.Any(x => x.Username == recipient.UserName))
            {
                message.DateRead = DateTime.UtcNow;
            } 
            else {
                //otherwise if user is at any other part or component of the application and not connected to the same message group when user sends the mesage,
                //allow them to receive a notification that they have a new message
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);
                
                if (connections != null)
                {   
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                     new {username = sender.UserName, knownAs = sender.KnownAs});
                } //if user is not connected to the presence hub, it will not receive notification
            }


            //Add message in order for entity framework to track
            _messageRepository.AddMessage(message);

            // save entity framework into database and pass through message dto from message. 
            if (await _messageRepository.SaveAllAsync())
            {   //send message to groups with particular groupname connected in hub.
                await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            } 
        }


        //sort in alphabetical order
        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        //updating message hub with group tracking after creating group and connection entites.
        //return group itself to see who is inside the group already and control who send the message thread back to.
        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _messageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, Context.User.GetUsername());

            if (group == null)
            {
                group = new Group(groupName);
                _messageRepository.AddGroup(group);
            }  

            group.Connections.Add(connection);

            if (await _messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to add to group");

        }

        //removing connection from database and not signalR
         private async Task<Group> RemoveFromMessageGroup()
         {
            var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);

            if (await _messageRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to remove from group");
         }

    }
}