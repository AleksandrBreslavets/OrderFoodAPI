using Microsoft.EntityFrameworkCore;
using OrderFoodAPIWebApp.Models;

namespace OrderFoodAPIWebApp.Models
{
    public class FoodOrderAPIContext: DbContext
    {
        public virtual DbSet<Category> Categories { get; set; }

        public virtual DbSet<City> Cities { get; set; }

        public virtual DbSet<Customer> Customers { get; set; }

        public virtual DbSet<Dish> Dishes { get; set; }

        public virtual DbSet<DishOrder> DishOrders { get; set; }

        public virtual DbSet<DishRestaurant> DishRestaurants { get; set;}

        public virtual DbSet<Order> Orders { get; set; }

        public virtual DbSet<Restaurant> Restaurants { get; set; }

        public virtual DbSet<Address> Addresses { get; set; }

        public FoodOrderAPIContext() { }

        public FoodOrderAPIContext(DbContextOptions<FoodOrderAPIContext> options)
        : base(options) 
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<DishOrder>(entity =>
            {
                entity.HasKey(e => new { e.OrderId, e.DishId });
                entity
                    .HasOne(dor => dor.Order)
                    .WithMany(o => o.DishOrders)
                    .HasForeignKey(dor => dor.OrderId);

                entity
                    .HasOne(dor => dor.Dish)
                    .WithMany(d => d.DishOrders)
                    .HasForeignKey(dor => dor.DishId);
            });

            modelBuilder.Entity<DishRestaurant>(entity =>
            {
                entity.HasKey(e => new { e.RestaurantId, e.DishId });
                entity
                    .HasOne(dr => dr.Restaurant)
                    .WithMany(r => r.DishRestaurants)
                    .HasForeignKey(dr => dr.RestaurantId);

                entity
                    .HasOne(dr => dr.Dish)
                    .WithMany(d => d.DishRestaurants)
                    .HasForeignKey(dr => dr.DishId);
            });

            modelBuilder.Entity<Dish>()
                .HasOne(d => d.Category)
                .WithMany(c => c.Dishes)
                .HasForeignKey(d => d.CategoryId);

            modelBuilder.Entity<Address>()
                .HasOne(r => r.City)
                .WithMany(c => c.Addresses)
                .HasForeignKey(r => r.CityId);

            modelBuilder.Entity<Restaurant>()
                .HasOne(r => r.Address)
                .WithMany(c => c.Restaurants)
                .HasForeignKey(r => r.AddressId);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(r => r.CreationTime).HasColumnType("datetime");

                entity
                    .HasOne(o => o.Customer)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(d => d.CustomerId);

                entity
                    .HasOne(o => o.Address)
                    .WithMany(c => c.Orders)
                    .HasForeignKey(d => d.AddressId);
            });
                
        }
        public DbSet<OrderFoodAPIWebApp.Models.Address> Address { get; set; } = default!;

    }
}
