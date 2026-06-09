using System;
using System.Collections.Generic;
using System.Text;

namespace _2.semEksamenProjekt
{
    public class User
    {
        public int Id { get; set; }  // Dennis
        public string Navn { get; set; }  // Dennis
        public string Username { get; set; } // Nicolas
        public string Password { get; set; } // Nicolas
        public string Role { get; set; } // Nicolas

        public override string ToString()  // Dennis
        {
            return $"{Navn} ({Username})";  // Dennis
        }
    }
}