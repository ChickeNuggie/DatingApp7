using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Connection
    {   //empty constructor for entity framework that takes no parameter values so it doesnt complain when try and create migration and schema for database.
        public Connection()
        {
            
        }
        public Connection(string connectionId, string username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        public string ConnectionId { get; set; }
    
        public string Username { get; set; }
    }
}