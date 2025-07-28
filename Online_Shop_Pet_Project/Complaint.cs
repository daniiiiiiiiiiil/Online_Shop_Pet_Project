using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Shop_Pet_Project
{
    public class Complaint
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone { get; set; }
        public string Subject { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public int OrderId { get; set; }
    }
}
