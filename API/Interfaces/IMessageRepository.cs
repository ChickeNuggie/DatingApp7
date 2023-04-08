using API.DTO;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message> GetMessage(int id);
        //Similar to unread message inbox and outbox, taking a parameter to see which container of messages the user wanats to view.
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        // returns type of List of messages between 2 individual users
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName);
        Task<bool> SaveAllAsync();
   }
}