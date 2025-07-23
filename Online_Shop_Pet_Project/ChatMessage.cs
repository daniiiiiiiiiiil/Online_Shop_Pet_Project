using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Shop_Pet_Project
{
    public class ChatMessage
    {
        public string Sender { get; set; }
        public string Text { get; set; }
        public DateTime Time { get; set; }
        public bool IsSupport { get; set; }
    }
}
