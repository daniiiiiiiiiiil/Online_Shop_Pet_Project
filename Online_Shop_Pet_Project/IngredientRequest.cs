using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Shop_Pet_Project
{
    public class IngredientRequest
    {
        public string IngredientName { get; set; }
        public double Quantity { get; set; }
        public string Unit { get; set; }
        public string Comment { get; set; }
        public DateTime RequestTime { get; set; }
        public string Status { get; set; }
    }
}
