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

        [Required(ErrorMessage = "Назва вулиці є обов'язковою!")]
        [MaxLength(50, ErrorMessage = "Введіть коротшу назву вулиці")]
        [Column(TypeName = "nvarchar(50)")]
        [Display(Name = "Вулиця")]
        public string StreetName { get; set; } = null!;

        [Display(Name = "Номер будівлі")]
        [Required(ErrorMessage = "Номер будівлі обов'язковий!")]
        [Range(1, int.MaxValue, ErrorMessage = "Значення повинно бути більше 0")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Номер будівлі повинен бути цілим додатнім числом")]
        public int BuldingNumber { get; set; }

        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Ціна повинна бути додатнім десятковим числом з максимально двома знаками після крапки")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Значення повинно бути більше 0")]
        [Display(Name = "Рейтинг")]
        public double? Rating { get; set; }

        [Display(Name = "Місто")]
        public virtual City City { get; set; } = null!;

        [Display(Name = "Місто")]
        [Required(ErrorMessage = "Місто обов'язкове!")]
        public int CityId { get; set; }

        [Display(Name = "Страви")]
        public virtual ICollection<DishRestaurant> DishRestaurants { get; set; }

    }
}
