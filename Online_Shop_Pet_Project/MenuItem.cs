using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Online_Shop_Pet_Project
{
    public class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Ingredients { get; set; }
        public int CookingTime { get; set; }
        public List<IngredientRequirement> RequiredIngredients { get; set; } = new List<IngredientRequirement>();
    }
}
