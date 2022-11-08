using AutoMapper;
using DatingApp.Commands;
using DatingApp.Data;
using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DatingApp.Controllers
{
    public class AccountController : BasicController
    {
        private readonly DataContext _context;
        private readonly ITokenService _token;
        private readonly IMediator _mediator;

        public AccountController(DataContext context, IMediator mediator,ITokenService token)
        {
            _context = context;
            _mediator = mediator;
            _token = token;
        }
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> Register(RegisterDTO register)
        {
            if (await UserExist(register.Username)) return BadRequest("Username already taken");

            //var user = _mapper.Map<AppUser>(register);
            //using var hmac = new HMACSHA512();

            //user.UserName = register.Username.ToLower();
            //user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(register.Password));
            //user.PasswordSalt = hmac.Key;
            var user = _mediator.Send(new RegisterUserCommand(register));

            _context.Add(user.Result);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Username = user.Result.UserName,
                Token = _token.CreateToken(user.Result),
                KnownAs = user.Result.KnownAs,
                Gender = user.Result.Gender

            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO login)
        {
            var user = await _context.Users
                .Include(p=>p.Photos)
                .SingleOrDefaultAsync(x => x.UserName==login.Username);

            if (user == null) return Unauthorized("User doesn't exist");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(login.Password));

            for(int i=0;i<computedHash.Length;i++)
            {
                if (user.PasswordHash[i] != computedHash[i]) return Unauthorized("Password doesn't match");
            }
            return new UserDTO
            {
                Username = user.UserName,
                Token = _token.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.isMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }
        private async Task<bool> UserExist(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }

}
