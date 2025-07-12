using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Shop_Pet_Project
{
    public class KitchenOrder
    {
        public int Id { get; set; }
        public int TableNumber { get; set; }
        public string Items { get; set; }
        public string Status { get; set; }
        public DateTime Time { get; set; }
    }
}
