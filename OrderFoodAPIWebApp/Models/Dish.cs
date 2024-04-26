using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderFoodAPIWebApp.Models
{
    public class Dish
    {
        public Dish()
        {
            DishRestaurants = new List<DishRestaurant>();
            DishOrders = new List<DishOrder>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва страви є обов'язковою!")]
        [MaxLength(50, ErrorMessage = "Введіть коротшу назву страви")]
        [Column(TypeName = "nvarchar(50)")]
        [Display(Name = "Страва")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Вага страви є обов'язковою!")]
        [Display(Name = "Вага страви")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Значення повинно бути більше 0")]
        public double Weight { get; set; }

        [Display(Name = "Опис")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Ціна страви є обов'язковою!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Значення повинно бути більше 0")]
        [Display(Name = "Ціна страви")]
        public double Price { get; set; }

        [Display(Name = "Кількість калорій")]
        [Required(ErrorMessage = "Кількість калорій обов'язкова!")]
        [Range(1, int.MaxValue, ErrorMessage = "Значення повинно бути більше 0")]
        //[RegularExpression(@"^\d+$", ErrorMessage = "Кількість калорій повинна бути цілим додатнім числом")]
        public int Calories { get; set; }

        [Display(Name = "Категорія")]
        [Required(ErrorMessage = "Категорія страви обов'язкова!")]
        public int CategoryId { get; set; }

        [Display(Name = "Категорія")]
        public virtual Category? Category { get; set; }

        [Display(Name = "Ресторани з цією старвою")]
        public virtual ICollection<DishRestaurant> DishRestaurants { get; set; }

        [Display(Name = "Замовлення з цією стравою")]
        public virtual ICollection<DishOrder> DishOrders { get; set; }
    }
}
