using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Shop_Pet_Project
{

    public class ChatTicket
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public List<ChatMessage> Messages { get; set; } = new List<ChatMessage>();
    }
}
