using BusinessObjectLayer;
using ExpenseSharing.Models;

public class UserLoginToUserHelper
{
    public User UserLoginToUserMapping(UserLogin login)
    {
        return new User
        {
            EmailId = login.Email,
            Password = login.Password
        };
    }
}
