using OrderFoodAPIWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CategoriesTests
{
    public class TestDataHelper
    {
        public static List<Category> GetFakeCategoriesList()
        {
            return new List<Category>()
            {
                new Category
                {
                    Id = 1,
                    Name = "Перші страви",
                    Dishes=new List<Dish>()

                },
                new Category
                {
                    Id = 2,
                    Name = "Напої",
                    Dishes=new List<Dish>()
                }
            };
        }
    }
}
