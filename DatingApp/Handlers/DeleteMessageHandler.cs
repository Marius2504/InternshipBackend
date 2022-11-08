using DatingApp.Commands;
using DatingApp.Data;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DatingApp.Handlers
{
    public class DeleteMessageHandler : IRequestHandler<DeleteMessageCommand,int>
    {
        private readonly MessageRepository _messageRepository;

        public DeleteMessageHandler(MessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<int> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
        {
            var id = request.id;
            var username = request.username;
            var message = await _messageRepository.GetMessages(id);
            if (message.Sender.UserName != username && username != message.Recipient.UserName)
                return 0;
            if (message.Sender.UserName == username) message.SenderDeleted = true;
            if (message.Recipient.UserName == username) message.RecipientDeleted = true;
            if (message.SenderDeleted && message.RecipientDeleted)
                _messageRepository.DeleteMessage(message);
            if (await _messageRepository.SaveAllAsync()) return 1;
            return 0;
        }
    }
}
