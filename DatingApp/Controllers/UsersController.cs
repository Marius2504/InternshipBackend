using AutoMapper;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Extensions;
using DatingApp.Helpers;
using DatingApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{ 
    [Authorize]
    public class UsersController : BasicController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _photoService = photoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers([FromQuery]UserParams userParams)
        {
            var user = await _userRepository.GetUserByNameAsync(User.GetUsername());
            userParams.CurrentUsername = User.GetUsername();
            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = user.Gender == "male" ? "female" : "male";

            var users = await _userRepository.GetMembersAsync(userParams); 
            Response.AddPaginationHeader(users.CurrentPge, users.PageSize,
                users.TotalCount, users.TotalPges);
            return Ok(users);

            
           // return Ok(_mapper.Map<IEnumerable<MemberDTO>>(users));
        }

        [HttpGet("{name}",Name ="GetUser")]
        [Authorize]
        public async Task<ActionResult<MemberDTO>> GetUser(string name)
        {
            return await _userRepository.GetMemberByNameAsync(name);
           // return _mapper.Map<MemberDTO>(user);
        }
        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDTO member)
        {
            var username = User.GetUsername();
            var user = await _userRepository.GetUserByNameAsync(username);

            _mapper.Map(member, user);

            _userRepository.Upate(user);
            if (_userRepository.SaveChangesAsync().Result) return NoContent();
            return BadRequest("Update failed");
        }
        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByNameAsync(User.GetUsername());
            var result = await _photoService.AddPhotoAsync(file);
            if(result.Error !=null)
            {
                return BadRequest(result.Error.Message);
            }
            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if(user.Photos.Count==0)
            {
                photo.isMain = true;
            }
            user.Photos.Add(photo);

            if(await _userRepository.SaveChangesAsync())
            {
                return CreatedAtAction("GetUser",new { name=user.UserName},_mapper.Map<PhotoDTO>(photo));
            }
            return BadRequest("didnt work well");
            

        }
        [HttpPut("set-main-photo/{photoId}")]
        public async Task<ActionResult> setMainPhoto(int photoId)
        {
            var user = await _userRepository.GetUserByNameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo.isMain) return BadRequest("This is already yor main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.isMain);
            if (currentMain != null) currentMain.isMain = false;
            photo.isMain = true;

            if (await _userRepository.SaveChangesAsync()) return NoContent();
            return BadRequest("failed to set main photo");
        }
        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> deletePhoto(int photoId)
        {
            var user = await _userRepository.GetUserByNameAsync(User.GetUsername());
            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

            if (photo == null) return BadRequest();
            
            if(photo.PublicId !=null)
            {
               var result= await _photoService.DeletePhotoAsync(photo.PublicId);
                if(result.Error !=null)
                {
                    return BadRequest();
                }
                user.Photos.Remove(photo);
            }
            if (await _userRepository.SaveChangesAsync()) return Ok();
            return BadRequest();
        }
          
    }
}
