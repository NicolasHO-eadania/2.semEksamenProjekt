using System;
using System.Collections.Generic;
using System.Text;

namespace _2.semEksamenProjekt.Services
{
    public class UserServiceAdapter : IUserService
    {
        public bool Login(string username, string password)
        {
            return Database.ValidateUser(username, password);
        }

        public string GetRole(string username, string password)
        {
            return Database.GetUserRole(username, password);
        }
    }
}
