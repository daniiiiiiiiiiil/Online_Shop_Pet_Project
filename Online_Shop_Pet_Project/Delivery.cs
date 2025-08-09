using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Shop_Pet_Project
{
    public class Delivery
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string Status { get; set; }
        public List<DeliveryStatus> StatusHistory { get; set; } = new List<DeliveryStatus>();
        public decimal Payment { get; set; }
        public List<string> OrderItems { get; set; }
    }
}
