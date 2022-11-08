using DatingApp.DTOs;
using DatingApp.Entities;
using DatingApp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp.Interfaces
{
    public interface IUserRepository
    {
        void Upate(AppUser appUser);
        Task<bool> SaveChangesAsync();
        Task<IEnumerable<AppUser>> GetUsersAsync();
        Task<AppUser> GetUserByIdAsync(int id);
        Task<AppUser> GetUserByNameAsync(string name);
        Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams);
        Task<MemberDTO> GetMemberByNameAsync(string name);

    }
}
