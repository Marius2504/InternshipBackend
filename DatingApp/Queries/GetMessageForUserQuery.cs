using DatingApp.DTOs;
using DatingApp.Helpers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Queries
{
    public record GetMessageForUserQuery(MessageParams MessageParams):IRequest<PagedList<MessageDTO>>;
}
