using DatingApp.Commands;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using DatingApp.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    public class LikesController : BasicController
    {
        private readonly Mediator _mediator;
        public LikesController(Mediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("{username}")]
        public async Task<ActionResult> AddLike(string username)
        {
            var sourceUserId = User.GetUserId();
            

            //if (sourceUser.UserName == username) return BadRequest();
            //if (likedUser == null) return NotFound();
            //var userLike = await _likeRepository.GetUserLike(sourceUserId, likedUser.Id);
            //if (userLike != null) 
            //    return BadRequest("You already like this user");

            //userLike = new LikeUser
            //{
            //    SourceUser = sourceUser,
            //    SourceUserId = sourceUserId,
            //    LikedUser = likedUser,
            //    LikedUserId = likedUser.Id
            //};
            var userLike = _mediator.Send(new AddLikeCommand(username, sourceUserId));
            //sourceUser.LikedUsers.Add(userLike.Result);
            //if (await _userRepository.SaveChangesAsync())
            //    return Ok();
            if (userLike.Result != null) return Ok();  
            return BadRequest("Failed to like user");
        }
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<LikeDTO>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = _mediator.Send(new GetUserLikesQuery(likesParams,likesParams.UserId));
            Response.AddPaginationHeader(users.Result.CurrentPge, users.Result.PageSize, users.Result.TotalCount, users.Result.TotalPges);

            return Ok(users);
        }
    }
}
