using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using API.DTO;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{   //After creating repository, entity(class) and data migration,
    //Create controller to use functionality inside the repository.
    //Http method created by API. (unique custom code created by users). Connects to database
    //Bundles all the api components into one for reuseability.
    //In this case, ability to create message
    //Note: BaseAPI include API controller attributes and route that uses controller

    public class MessagesController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _uow;

        public MessagesController(IUnitOfWork uow,
         IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        [HttpPost] // connector to database('html')
        public async Task<ActionResult<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var username = User.GetUsername();

            if (username == createMessageDto.RecipientUsername.ToLower())
                return BadRequest("You cannot send messages to yourself");

            var sender = await _uow.UserRepository.GetUserByUsernameAsync(username);
            var recipient = await _uow.UserRepository.GetUserByUsernameAsync(createMessageDto.RecipientUsername);            
        
            if (recipient == null) return NotFound();

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content
            };
            //Add message in order for entity framework to track
            _uow.MessageRepository.AddMessage(message);

            // save entity framework into database and pass through message dto from message. 
            if (await _uow.Complete()) return Ok(_mapper.Map<MessageDto>(message)); 
        
            return BadRequest("Failed to send message");
        }

        [HttpGet] // connector to database('html')
        public async Task<ActionResult<PagedList<MessageDto>>> GetMessagesForUser([FromQuery]MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();

            var messages = await _uow.MessageRepository.GetMessagesForUser(messageParams);

            Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize,
            messages.TotalCount, messages.TotalPages));

             return messages;
        }

        //delete the id of the message that users are interested in deleting.
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();

            var message = await _uow.MessageRepository.GetMessage(id);

            //Ensure that messages is deleted after both sender and receiver deletes the messages.
            if (message.SenderUsername != username && message.RecipientUsername != username) 
                return Unauthorized();


            if (message.SenderUsername == username) message.SenderDeleted = true;
            if (message.RecipientUsername == username) message.RecipientDeleted = true;

            if (message.SenderDeleted && message.RecipientDeleted)
            {
                _uow.MessageRepository.DeleteMessage(message);
            }

            //Update database if changes has been made.
            if(await _uow.Complete()) return Ok();
            
            return BadRequest("Problem deleting the message");
        
        }



    }
}