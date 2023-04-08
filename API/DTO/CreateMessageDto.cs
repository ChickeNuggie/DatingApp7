namespace API.DTO
{   //Receive message from the clients
    public class CreateMessageDto
    {
     public string RecipientUsername { get; set; }   
    
    public string Content { get; set; }
    }
}