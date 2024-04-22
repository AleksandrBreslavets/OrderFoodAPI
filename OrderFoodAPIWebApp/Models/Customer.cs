using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderFoodAPIWebApp.Models
{
    public class Customer
    {
        public Customer()
        {
            Orders = new List<Order>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Ім'я є обов'язковим!")]
        [MaxLength(50, ErrorMessage = "Введіть коротше ім'я")]
        [Column(TypeName = "nvarchar(50)")]
        [Display(Name = "Ім'я замовника")]
        public string CustomerName { get; set; } = null!;

        [Display(Name = "Номер телефону")]
        [Required(ErrorMessage = "Номер телефону є обов'язковим!")]
        [RegularExpression(@"^\+(?:[0-9] ?){6,14}[0-9]$", ErrorMessage = "Телефон має починатися з +, мати код країни та від 6 до 14 цифр.")]
        [Column(TypeName = "nvarchar(50)")]
        public string CustomerPhone { get; set; } = null!;

        [Display(Name = "Замовлення")]
        public virtual ICollection<Order> Orders { get; set; } 
    }
}
