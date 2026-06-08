using System;
using System.Collections.Generic;
using System.Text;

namespace _2.semEksamenProjekt.Services
{
    public interface IUserService
    {
        bool Login(string username, string password);
        string GetRole(string username, string password);
    }
}
