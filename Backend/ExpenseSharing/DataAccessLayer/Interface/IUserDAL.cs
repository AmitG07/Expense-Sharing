using BusinessObjectLayer;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataAccessLayer.Interface
{
    public interface IUserDAL
    {
        User GetUserByEmailAndPassword(string email, string password);
        User GetUserById(int userId);
        IEnumerable<User> GetAllUsers();
        Task<User> GetUserByIdAsync(int userId);
        Task UpdateUser(User user);
    }
}