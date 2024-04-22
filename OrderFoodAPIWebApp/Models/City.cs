using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderFoodAPIWebApp.Models
{
    public class City
    {
        public City()
        {
            Restaurants = new List<Restaurant>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва міста є обов'язковою!")]
        [MaxLength(50, ErrorMessage = "Введіть коротшу назву міста")]
        [Column(TypeName = "nvarchar(50)")]
        [Display(Name = "Місто")]
        public string Name { get; set; } = null!;

        [Display(Name = "Ресторани")]
        public virtual ICollection<Restaurant> Restaurants { get; set; } 
    }
}
