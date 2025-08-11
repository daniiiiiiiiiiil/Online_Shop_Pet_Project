using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Shop_Pet_Project
{
    public static class CurrentUser
    {
        public static int Id { get; set; }
        private static string username;
        private static bool isEmployee;
        public static UserProfile Profile { get; set; }
        public static string Username
        {
            get => username;
            set => username = value; 
        }

        public static bool IsEmployee
        {
            get => isEmployee;
            set => isEmployee = value;
        }

        public static void Clear()
        {
            Id = 0;
            Username = null;
            IsEmployee = false;
            Profile = null;
        }
    }
}
