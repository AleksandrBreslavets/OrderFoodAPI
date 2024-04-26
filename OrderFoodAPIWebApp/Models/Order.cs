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

        [Required(ErrorMessage = "Вартість доставки є обов'язковою!")]
        //[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Вартість доставки повинна бути додатнім десятковим числом з максимально двома знаками після крапки")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Значення повинно бути більше 0")]
        [Display(Name = "Вартість доставки")]
        public double DeliveryCost { get; set; }

        [Required(ErrorMessage = "Загальна вартість замовлення є обов'язковою!")]
        //[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Загальна вартість замовлення повинна бути додатнім десятковим числом з максимально двома знаками після крапки")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Значення повинно бути більше 0")]
        [Display(Name = "Загальна вартість замовлення")]
        public double TotalCost { get; set; }

        [Display(Name = "Дата та час створення замовлення")]
        //[Required(ErrorMessage = "Дата та час створення замовлення є обов'язковими!")]
        public DateTime? CreationTime { get; set; }

        [Required(ErrorMessage = "Оберіть чи замовлення виконане!")]
        [Display(Name = "Чи виконане замовлення")]
        public bool IsReady { get; set; }

        [Display(Name = "Замовник")]
        [Required(ErrorMessage = "Замовник обов'язковий!")]
        public int CustomerId { get; set; }

        [Display(Name = "Замовник")]
        public virtual Customer? Customer { get; set; } 

        [Display(Name = "Адреса доставки")]
        public virtual Address? Address { get; set; }

        [Display(Name = "Адреса доставки")]
        [Required(ErrorMessage = "Адреса доставки обов'язкова!")]
        public int AddressId { get; set; }

        [Display(Name = "Страви")]
        public virtual ICollection<DishOrder> DishOrders { get; set; } 
    }
}
