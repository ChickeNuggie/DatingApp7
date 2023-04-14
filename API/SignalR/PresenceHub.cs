using API.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _tracker;
        public PresenceHub(PresenceTracker tracker)
        {
            _tracker = tracker;
            
        }
        // notify users when a specific user connects to the same hub as them
        //get information when connected or disconencted to hub but does not track information on who ic curerntly connected to this signal
        public override async Task OnConnectedAsync()
        
        {   
            //track users who are connected to the hub
            var isOnline = await _tracker.UserConnected(Context.User.GetUsername(), Context.ConnectionId);
            
            //only notify clients that are connected if user has "just come" online based on logic inside presence tracker.
            if (isOnline) 
                //get and send username to other users' client (any other clients connected to this hub) to notify that they online.
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUsername());

            //get lists of currently online users
            var currentUsers = await _tracker.GetOnlineUsers();
            //send back to all connected clients when somebody connects.
            //This allows clients connected to app to update their list of who is currently online by display info in the browser.
            //Send to only curently online users (only calling clients gets the full lists of online users)
            await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
        }   

        //when user disconnect the hub
        public override async Task OnDisconnectedAsync(Exception exception) 
        {

            var isOffline = await _tracker.UserDisconnected(Context.User.GetUsername(), Context.ConnectionId);
            
            //If users gone offline, only then will we notify the other clients sthat user has gone offline.
            if (isOffline)
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUsername());
        
            //required when passing exception as parameter.
            await base.OnDisconnectedAsync(exception); 
        }
    }
}