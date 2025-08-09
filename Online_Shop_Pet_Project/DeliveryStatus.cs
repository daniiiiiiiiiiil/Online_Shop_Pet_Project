using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Shop_Pet_Project
{
    public class DeliveryStatus
    {
        public string Status { get; set; }
        public DateTime Timestamp { get; set; }

        public DeliveryStatus(string status, DateTime timestamp)
        {
            Status = status;
            Timestamp = timestamp;
        }
    }
}
