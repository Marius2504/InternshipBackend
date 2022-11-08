using AutoMapper;
using DatingApp.Commands;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    [Authorize]
    public class MessagesController : BasicController
    {
        private readonly IMediator _mediator;
        private readonly IMessageRepository _messageRepository;

        public MessagesController(IMediator mediator,IMessageRepository messageRepository)
        {
            _mediator = mediator;
            _messageRepository = messageRepository;
        }
        [HttpPost]
        public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO)
        {
            var username = User.GetUsername();
            if (username == createMessageDTO.RecipientUsername.ToLower())
                return BadRequest("You can't message yourself");
            /*
            var sender = await _userRepository.GetUserByNameAsync(username);
            var recipient = await _userRepository.GetUserByNameAsync(createMessageDTO.RecipientUsername);
            if (recipient == null) return NotFound();
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDTO.Content
            };
            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDTO>(message));
            return BadRequest("Failed to send the message");
            */
            return await _mediator.Send(new CreateMessageCommand(username, createMessageDTO));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageForUser([FromQuery] 
        MessageParams messageParams)
        {
            messageParams.Username = User.GetUsername();
            // var messages = await _messageRepository.GetMessagesForUser(messageParams);
            var messages = await _mediator.Send(new GetMessageForUserQuery(messageParams));
            Response.AddPaginationHeader(messages.CurrentPge,messages.PageSize,messages.TotalCount,
                messages.TotalPges);
            return messages;
        }
        [HttpGet("thread/{username}")]
        public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username)
        {
            var currentUser = User.GetUsername();
            return Ok(await _messageRepository.GetMessageThread(currentUser, username));
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMessage(int id)
        {
            var username = User.GetUsername();
            if( await _mediator.Send(new DeleteMessageCommand(id, username)) ==0)
            {
                return BadRequest("Something went wrong");
            }
            return Ok();
            /*
            var message = await _messageRepository.GetMessages(id);
            if (message.Sender.UserName != username && username != message.Recipient.UserName)
                return Unauthorized();
            if (message.Sender.UserName == username) message.SenderDeleted = true;
            if (message.Recipient.UserName == username) message.RecipientDeleted = true;
            if (message.SenderDeleted && message.RecipientDeleted) 
                _messageRepository.DeleteMessage(message);
            if (await _messageRepository.SaveAllAsync()) return Ok();
            return BadRequest("Something went wrong");
            */
        }
    }
}
