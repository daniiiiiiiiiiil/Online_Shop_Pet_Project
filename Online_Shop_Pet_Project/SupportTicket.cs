using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Shop_Pet_Project
{
    public class SupportTicket
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Answer { get; set; }
    }
}
