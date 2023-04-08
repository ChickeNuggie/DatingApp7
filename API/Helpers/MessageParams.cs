namespace API.Helpers
{   
    //Paginated parameters to select which container to read the message (inbox/outbox)
    public class MessageParams : PaginationParams
    {
        public string Username { get; set; }
    
        public string Container { get; set; } = "Unread";
    }
}