using API.DTO;
using API.Entities;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        void Update(User user);
        Task<bool> SaveAllChanges();
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);



        Task<IEnumerable<MemberDto>> GetMembersAsync();
        Task<MemberDto?> GetMemberAsync(string username);

    }

}
