using DatingApp.DTOs;
using DatingApp.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Queries
{
    public record CreateMessageCommand(string username,CreateMessageDTO message):IRequest<MessageDTO>;
}
