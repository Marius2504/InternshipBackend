using DatingApp.DTOs;
using DatingApp.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Commands
{
    public record RegisterUserCommand(RegisterDTO register) :IRequest<AppUser>;
   
}
