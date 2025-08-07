using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Shop_Pet_Project
{
    public static class CurrentUser
    {
        private static string username;
        private static bool isEmployee;

        public static string Username
        {
            get => username;
            set => username = value ?? throw new ArgumentNullException(nameof(value));
        }

        public static bool IsEmployee
        {
            get => isEmployee;
            set => isEmployee = value;
        }

        public static void Clear()
        {
            username = null;
            isEmployee = false;
        }
    }
}
