using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Interfaces;
using DatingApp.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DatingApp.Commands
{
    public class CreateMessageHandler : IRequestHandler<CreateMessageCommand, MessageDTO>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateMessageHandler(IMessageRepository messageRepository, IUserRepository userRepository,
            IMapper mapper)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<MessageDTO> Handle(CreateMessageCommand request, CancellationToken cancellationToken)
        {
            var sender = await _userRepository.GetUserByNameAsync(request.username);
            var recipient = await _userRepository.GetUserByNameAsync(request.message.RecipientUsername);
            if (recipient == null) return null;
            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = request.message.Content
            };
            _messageRepository.AddMessage(message);
            if (await _messageRepository.SaveAllAsync()) return _mapper.Map<MessageDTO>(message);
            return null;
        }
    }
}
