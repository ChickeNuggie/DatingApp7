namespace API.SignalR
{
    public class PresenceTracker
    {
        //values of username and list of connection ID for the particular user(login from multiple devices)
        private static readonly Dictionary<string, List<string>> OnlineUsers = 
            new Dictionary<string, List<string>>();

        //return boolean to specify if user is genuinely just come online (didnt have any other conenctions or is genuinely gone offline)
        public Task<bool> UserConnected(string username, string connectionId)
        {   
            bool isOnline =  false;
            
            //lock onlineusers while on connecting to this dictionary
            //anyone else that's connecting have ti wait their turn to be addedd into the dictionary
            //prevent multiple concurrent users trying to access dictionary at the same time. (not thread safety)
            lock(OnlineUsers)
            {   
                //check if user is connected in dictionary
                //if user already got a connection and simply adding connection id to the key of username, then the user haven't geuniely come online
                //as they just added another connection and were already online before
                if (OnlineUsers.ContainsKey(username))
                {
                    //add new connection to dictionary
                    OnlineUsers[username].Add(connectionId);
                }
                else 
                {   //if adding new entry in list, set isOnline property to true. (genuinely online)
                    OnlineUsers.Add(username, new List<string>{connectionId});
                    isOnline = true;
                }
            }

            return Task.FromResult(isOnline);
        }

        public Task<bool> UserDisconnected(string username, string connectionId)
        {
            bool isOffline = false;

            lock(OnlineUsers)
            {
                if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);
                //only remove from dictionary if not at specific hub (i.e message hub but presence hub)
                OnlineUsers[username].Remove(connectionId);
                //if key of username count is 0, we know that user has actually gone offline from the presence connection hub.
                if (OnlineUsers[username].Count == 0)
                {
                    OnlineUsers.Remove(username);
                    isOffline = true;
                }
            }

            return Task.FromResult(isOffline);
        }

        //Get information on who is online and return this information to other users where they can see who is online when they connect.
        public Task<string[]> GetOnlineUsers()
        {
            string[] onlineUsers;
            lock(OnlineUsers)
            {   //get lists of key = usersname by alphabetical order
                onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
            }

            return Task.FromResult(onlineUsers); // return online users.
        }

        //Get connection for specific user from multiple devices in order to send notification to every one of their connection upon receiving messages.
        public static Task<List<string>> GetConnectionsForUser(string username)
        {
            List<string> connectionIds;
            //avoid two or more concurrent users to access the list of connectionIds simulteaneously
            lock(OnlineUsers)
            {   //return list of connection for that particular users
                connectionIds = OnlineUsers.GetValueOrDefault(username);
            }

            return Task.FromResult(connectionIds);
        }

    }
}