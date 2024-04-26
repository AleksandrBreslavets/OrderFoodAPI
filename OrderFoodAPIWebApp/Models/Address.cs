using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderFoodAPIWebApp.Models
{
    public class Address
    {
        public Address()
        {
            Restaurants = new List<Restaurant>();
            Orders = new List<Order>();
        }

        [Key]
        public int Id { get; set; }

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

        [Display(Name = "Місто")]
        public virtual City? City { get; set; }

        [Display(Name = "Місто")]
        [Required(ErrorMessage = "Місто обов'язкове!")]
        public int CityId { get; set; }

        [Display(Name = "Страви")]
        public virtual ICollection<Restaurant> Restaurants { get; set; }

        [Display(Name = "Замовлення")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
