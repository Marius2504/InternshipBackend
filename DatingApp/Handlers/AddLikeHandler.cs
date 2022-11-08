using DatingApp.Commands;
using DatingApp.Entities;
using DatingApp.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DatingApp.Handlers
{
    public class AddLikeHandler : IRequestHandler<AddLikeCommand, LikeUser>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILikeRepository _likeRepository;

        public AddLikeHandler(IUserRepository userRepository, ILikeRepository likeRepository)
        {
            _userRepository = userRepository;
            _likeRepository = likeRepository;
        }
        public async Task<LikeUser> Handle(AddLikeCommand request, CancellationToken cancellationToken)
        {
            var likedUser = await _userRepository.GetUserByNameAsync(request.username);
            var sourceUser = await _likeRepository.GetUserWithLikes(request.sourceUserId);
            if (sourceUser.UserName == request.username) return null;
            if (likedUser == null) return null;
            var userLike = await _likeRepository.GetUserLike(request.sourceUserId, likedUser.Id);
            if (userLike != null)
                return null;

            userLike = new LikeUser
            {
                SourceUser = sourceUser,
                SourceUserId = request.sourceUserId,
                LikedUser = likedUser,
                LikedUserId = likedUser.Id
            };
            sourceUser.LikedUsers.Add(userLike);
            if (await _userRepository.SaveChangesAsync())
                return userLike;
            return null;
        }
    }
}
