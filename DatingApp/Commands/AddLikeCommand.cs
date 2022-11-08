using DatingApp.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Commands
{
    public record AddLikeCommand(string username,int sourceUserId) :IRequest<LikeUser>;
    
    
}
