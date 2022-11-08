using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Commands
{
    public record DeleteMessageCommand(int id,string username):IRequest<int>;
}
