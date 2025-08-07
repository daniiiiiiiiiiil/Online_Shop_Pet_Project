using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Shop_Pet_Project
{
    internal class EmployeeItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Position { get; set; }
        public override string ToString() => Name;
    }
}
