using System.ComponentModel.DataAnnotations;

namespace OrderFoodAPIWebApp.Models
{
    public class DishOrder
    {
        [Display(Name = "Кількість порцій страви")]
        [Required(ErrorMessage = "Кількість порцій страви обов'язкова!")]
        [Range(1, int.MaxValue, ErrorMessage = "Значення повинно бути додатнім")]
        //[RegularExpression(@"^\d+$", ErrorMessage = "Кількість порцій страви повинна бути цілим додатнім числом")]
        public int DishQuantity { get; set; }

        [Display(Name = "Страва")]
        [Required(ErrorMessage = "Страва обов'язкова!")]
        public int DishId { get; set; }

        [Display(Name = "Замовлення")]
        [Required(ErrorMessage = "Замовлення обов'язкове!")]
        public int OrderId { get; set; }

        [Display(Name = "Замовлення")]
        public virtual Order? Order { get; set; }

        [Display(Name = "Страва")]
        public virtual Dish? Dish { get; set; }
    }
}
