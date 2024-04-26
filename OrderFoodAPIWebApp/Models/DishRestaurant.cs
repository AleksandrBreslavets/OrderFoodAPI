using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderFoodAPIWebApp.Models
{
    public class DishRestaurant
    {
        [Required(ErrorMessage = "Оберіть чи є страва в наявності!")]
        [Display(Name = "Чи страва в наявності")]
        public bool IsDishAvailable { get; set; }

        [Display(Name = "Ресторан")]
        [Required(ErrorMessage = "Ресторан обов'язковий!")]
        public int RestaurantId { get; set; }

        [Display(Name = "Страва")]
        [Required(ErrorMessage = "Страва обов'язкова!")]
        public int DishId { get; set; }

        [Display(Name = "Ресторан")]
        public virtual Restaurant? Restaurant { get; set; }

        [Display(Name = "Страва")]
        public virtual Dish? Dish { get; set; } 
    }
}
