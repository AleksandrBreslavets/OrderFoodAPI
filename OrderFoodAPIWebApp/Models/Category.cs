using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderFoodAPIWebApp.Models
{
    public class Category
    {
        public Category() 
        {
            Dishes = new List<Dish>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва категорії є обов'язковою!")]
        [MaxLength(50, ErrorMessage = "Введіть коротшу назву категорії")]
        [Column(TypeName = "nvarchar(50)")]
        [Display(Name = "Категорія")]
        public string Name { get; set; } = null!;

        [Display(Name = "Страви")]
        public virtual ICollection<Dish> Dishes { get; set; } 
    }
}
