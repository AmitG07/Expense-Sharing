using BusinessObjectLayer;
using ExpenseSharing.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExpenseSharing.Helper
{
    public class UserModelToUserHelper
    {
        public User UserModelToUserMapping(UserModel e)
        {
            User u = new User();
            u.Name = e.Name;
            u.EmailId = e.EmailId;
            u.Password = e.Password;
            u.AvailableBalance = e.AvailableBalance;
            return u;
        }
    }
}
