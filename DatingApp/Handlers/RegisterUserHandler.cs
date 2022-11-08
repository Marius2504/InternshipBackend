using AutoMapper;
using DatingApp.Commands;
using DatingApp.DTOs;
using DatingApp.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DatingApp.Handlers
{
    public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, AppUser>
    {
        private readonly IMapper _mapper;

        public RegisterUserHandler(IMapper mapper)
        {
            _mapper = mapper;
        }
        public Task<AppUser> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var user = _mapper.Map<AppUser>(request.register);
            using var hmac = new HMACSHA512();

            user.UserName = request.register.Username.ToLower();
            user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.register.Password));
            user.PasswordSalt = hmac.Key;

            return Task.FromResult(user);
            
        }

    }
}
