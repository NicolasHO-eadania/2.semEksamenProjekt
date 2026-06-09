using System;
using System.Collections.Generic;
using System.Text;

namespace _2.semEksamenProjekt.Services
{
    public class UserServiceAdapter : IUserService
    {
        public bool Login(string username, string password)
        {
            return UserService.ValidateUser(username, password);
        }

        public string GetRole(string username, string password)
        {
            return UserService.GetUserRole(username, password);
        }
    }
}