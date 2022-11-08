using DatingApp.DTOs;
using DatingApp.Helpers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Queries
{
    public record GetUsersQuery(UserParams UserParams):IRequest<PagedList<MemberDTO>>;
    
    
}
