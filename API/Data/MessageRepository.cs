using API.DTO;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public MessageRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
            
        }

        public void AddMessage(Message message)
        {
            _context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            _context.Messages.Remove(message);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FindAsync(id);
        }

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _context.Messages
            .OrderByDescending(x => x.MessageSent)
            .AsQueryable();

            // switch statement to see which container(inbox/outbox) user wish to view the message for using query
            query = messageParams.Container switch
            {
                // delete on sender/receiver database side upon deletion.
                "Inbox" => query.Where(u => u.RecipientUsername == messageParams.Username 
                && u.RecipientDeleted == false),
                "Outbox" => query.Where(u => u.SenderUsername == messageParams.Username 
                && u.SenderDeleted == false),
                //unread messages
                _ => query.Where(u => u.RecipientUsername == messageParams.Username 
                && u.RecipientDeleted == false && u.DateRead == null) 
            };

            //Project query message to message dto
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            // return as a pagedlist.
            return await PagedList<MessageDto>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        
        }

        // Display messages between 2 users. (messages from currently logged in user to target user thay they want to message/messaged)
        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
        {
            var messages = await _context.Messages
            .Include(u => u.Sender).ThenInclude(p => p.Photos)// photos are related entity for app user.
            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
            .Where(
                m => m.RecipientUsername == currentUserName && m.RecipientDeleted == false && // delete from receiver side in messages tab.
                m.SenderUsername == recipientUserName ||
                m.RecipientUsername == recipientUserName && m.SenderDeleted == false && // delete from sender side in messages tab.
                m.SenderUsername == currentUserName
            )
            .OrderBy(m => m.MessageSent)
            .ToListAsync(); // return list of messages in memory as executed query against database

            //ensure that it is the recipient username that read the messages sent by sender and that's the message to mark as date read.
            var unreadMessages = messages.Where(m => m.DateRead == null &&
             m.RecipientUsername == currentUserName).ToList();

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {   // check and mark if there's any message read as soon as its received by recipient in unreadMessages, and update the date time.
                    //i.e. open message tab and see messages that are either read or unread instead of marked all read.
                    message.DateRead = DateTime.UtcNow; 
                }
                //save changes to database
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }

        public async Task<bool> SaveAllAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}