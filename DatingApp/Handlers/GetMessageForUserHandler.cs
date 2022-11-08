using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DatingApp.Handlers
{
    public class GetMessageForUserHandler : IRequestHandler<GetMessageForUserQuery, PagedList<MessageDTO>>
    {
        private readonly MessageRepository _messageRepository;

        public GetMessageForUserHandler(MessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }
        public async Task<PagedList<MessageDTO>> Handle(GetMessageForUserQuery request, CancellationToken cancellationToken)
        {
            return await _messageRepository.GetMessagesForUser(request.MessageParams);
        }
    }
}
