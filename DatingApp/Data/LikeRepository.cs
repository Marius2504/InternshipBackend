using AutoMapper;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Data
{
    public class LikeRepository : ILikeRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _imapper;

        public LikeRepository(DataContext context,IMapper imapper)
        {
            _context = context;
            _imapper = imapper;
        }
        public async Task<LikeUser> GetUserLike(int sourceUserId, int likedUserId)
        {
            var request = await _context.Likes.FindAsync(sourceUserId, likedUserId);
            return request;
        }

        public async Task<PagedList<LikeDTO>> GetUserLikes(LikesParams likesParams)
        {
            var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _context.Likes.AsQueryable();

            if(likesParams.Predicate =="liked")
            {
                likes = likes.Where(like => like.SourceUserId == likesParams.UserId);
                users = likes.Select(like => like.LikedUser);
            }
            if(likesParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likesParams.UserId);
                users = likes.Select(like => like.SourceUser);
            }
            var userLikeDTO =  users.Select(user => new LikeDTO
            {
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                Age = user.DateOfBirth.CalculateAge(),
                City = user.City,
                PhotoUrl = user.Photos.FirstOrDefault(p => !p.isMain).Url,
                Id = user.Id
            });
            return await PagedList<LikeDTO>.CreateAsync(userLikeDTO,likesParams.PageNumber,
                likesParams.PageSize);
        }


        public async Task<AppUser> GetUserWithLikes(int userId)//lista de useri la care a dat like
        {
            return await _context.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(s => s.Id == userId);
        }

    }
}
