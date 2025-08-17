using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Shop_Pet_Project
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhotoPath { get; set; }
        public string Address { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsSupport { get; set; }
        public string PaymentMethod { get; set; }
    }
}
