using BusinessLayer.Interface;
using BusinessObjectLayer;
using DataAccessLayer.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class UserBL : IUserBL
    {
        private readonly IUserDAL _userDAL;

        public UserBL(IUserDAL userDAL)
        {
            _userDAL = userDAL;
        }

        public User Authenticate(string email, string password)
        {
            return _userDAL.GetUserByEmailAndPassword(email, password);
        }

        public User GetUserById(int userId)
        {
            return _userDAL.GetUserById(userId);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _userDAL.GetAllUsers();
        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _userDAL.GetUserByIdAsync(userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user), "User object cannot be null");
            }
            await _userDAL.UpdateUser(user);
        }
    }
}
