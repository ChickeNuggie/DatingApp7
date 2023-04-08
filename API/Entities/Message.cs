namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId  { get; set; }
        public string SenderUsername { get; set; }
        public AppUser Sender { get; set; }
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public AppUser Recipient { get; set; }
        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;
        //if sender of a message decides to delete message from their side/view of messages, they dont have
        //control over inbox of other user that they sent their message to. 
        //provided both sender and receiver delete the message, it will be deleted from database
        public bool SenderDeleted { get; set; }
        public bool RecipientDeleted { get; set; }
         
        
    }
}