using DatingApp.DTOs;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DatingApp.Handlers
{
    public class GetUserLikesHandler : IRequestHandler<GetUserLikesQuery, PagedList<LikeDTO>>
    {
        private readonly ILikeRepository _likeRepository;

        public GetUserLikesHandler(ILikeRepository likeRepository)
        {
            _likeRepository = likeRepository;
        }
        public async Task<PagedList<LikeDTO>> Handle(GetUserLikesQuery request, CancellationToken cancellationToken)
        {
            var users = await _likeRepository.GetUserLikes(request.likesParams);
            return users;
        }
    }
}
