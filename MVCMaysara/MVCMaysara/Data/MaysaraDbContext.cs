using Microsoft.EntityFrameworkCore;
using MVCMaysara.Models;
using MVCMaysara.Models.Enums;

namespace MVCMaysara.Data
{
    public class MaysaraDbContext : DbContext
    {
        public MaysaraDbContext(DbContextOptions<MaysaraDbContext> options)
            : base(options)
        {
        }

        // DbSet properties for each table
        public DbSet<User> Users { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure User entity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("USERS");
                entity.HasKey(e => e.UserID);

                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.Role)
                    .HasConversion(
                        v => v.ToString(),
                        v => (UserRole)Enum.Parse(typeof(UserRole), v)
                    );
            });

            // Configure Restaurant entity
            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.ToTable("RESTAURANTS");
                entity.HasKey(e => e.RestaurantID);

                entity.HasOne(e => e.Owner)
                    .WithMany()
                    .HasForeignKey(e => e.OwnerID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.Products)
                    .WithOne(e => e.Restaurant)
                    .HasForeignKey(e => e.RestaurantID)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.Property(e => e.Rating).HasColumnType("decimal(3,2)");
                entity.Property(e => e.DeliveryFee).HasColumnType("decimal(10,2)");
            });

            // Configure Product entity
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("PRODUCTS");
                entity.HasKey(e => e.ProductID);

                entity.Property(e => e.Price).HasColumnType("decimal(10,2)");
            });

            // Configure Order entity
            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("ORDERS");
                entity.HasKey(e => e.OrderID);

                entity.Property(e => e.Status)
                    .HasConversion(
                        v => v.ToString(),
                        v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v)
                    );

                entity.Property(e => e.PaymentMethod)
                    .HasConversion(
                        v => v.ToString(),
                        v => (PaymentMethod)Enum.Parse(typeof(PaymentMethod), v)
                    );

                entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");

                entity.HasOne(e => e.User)
                    .WithMany()
                    .HasForeignKey(e => e.UserID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Restaurant)
                    .WithMany()
                    .HasForeignKey(e => e.RestaurantID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(e => e.OrderItems)
                    .WithOne(e => e.Order)
                    .HasForeignKey(e => e.OrderID)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure OrderItem entity
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("ORDER_ITEMS");
                entity.HasKey(e => e.OrderItemID);

                entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
                entity.Property(e => e.Subtotal).HasColumnType("decimal(10,2)");
            });
        }
    }
}
