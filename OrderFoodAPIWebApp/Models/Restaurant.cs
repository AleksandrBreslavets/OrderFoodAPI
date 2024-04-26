using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderFoodAPIWebApp.Models
{
    public class Restaurant
    {
        public Restaurant()
        {
            DishRestaurants = new List<DishRestaurant>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва ресторану є обов'язковою!")]
        [MaxLength(50, ErrorMessage = "Введіть коротшу назву ресторану")]
        [Column(TypeName = "nvarchar(50)")]
        [Display(Name = "Ресторан")]
        public string Name { get; set; } = null!;

        [Range(0.01, 5, ErrorMessage = "Значення повинно бути в межах від 0 до 5")]
        [Display(Name = "Рейтинг")]
        public double? Rating { get; set; }

        [Display(Name = "Адреса")]
        public virtual Address? Address { get; set; }

        [Display(Name = "Адреса")]
        [Required(ErrorMessage = "Адреса обов'язкова!")]
        public int AddressId { get; set; }

        [Display(Name = "Страви")]
        public virtual ICollection<DishRestaurant> DishRestaurants { get; set; }

    }
}
