using BusinessObjectLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer.Interface
{
    public interface IUserBL
    {
        User Authenticate(string email, string password);
        User GetUserById(int userId);
        IEnumerable<User> GetAllUsers();
        Task<User> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(User user);
    }
}
