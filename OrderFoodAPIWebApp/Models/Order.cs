using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderFoodAPIWebApp.Models
{
    public class Order
    {
        public Order()
        {
            DishOrders = new List<DishOrder>();
        }

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Назва вулиці є обов'язковою!")]
        [MaxLength(50, ErrorMessage = "Введіть коротшу назву вулиці")]
        [Column(TypeName = "nvarchar(50)")]
        [Display(Name = "Вулиця доставки")]
        public string DeliveryStreet { get; set; } = null!;

        [Display(Name = "Номер будівлі доставки")]
        [Required(ErrorMessage = "Номер будівлі обов'язковий!")]
        [Range(1, int.MaxValue, ErrorMessage = "Значення повинно бути більше 0")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Номер будівлі повинен бути цілим додатнім числом")]
        public int DeliveryBuilding { get; set; }

        [Required(ErrorMessage = "Вартість доставки є обов'язковою!")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Вартість доставки повинна бути додатнім десятковим числом з максимально двома знаками після крапки")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Значення повинно бути більше 0")]
        [Display(Name = "Вартість доставки")]
        public double DeliveryCost { get; set; }

        [Required(ErrorMessage = "Загальна вартість замовлення є обов'язковою!")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Загальна вартість замовлення повинна бути додатнім десятковим числом з максимально двома знаками після крапки")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Значення повинно бути більше 0")]
        [Display(Name = "Загальна вартість замовлення")]
        public double TotalCost { get; set; }

        [Display(Name = "Дата та час створення замовлення")]
        [Required(ErrorMessage = "Дата та час створення замовлення є обов'язковими!")]
        public DateTime CreationTime { get; set; }

        [Display(Name = "Замовник")]
        [Required(ErrorMessage = "Замовник обов'язковий!")]
        public int CustomerId { get; set; }

        [Display(Name = "Замовник")]
        public virtual Customer Customer { get; set; } = null!;

        [Display(Name = "Страви")]
        public virtual ICollection<DishOrder> DishOrders { get; set; } 
    }
}
