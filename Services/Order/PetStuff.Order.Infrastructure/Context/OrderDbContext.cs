using Microsoft.EntityFrameworkCore;
using OrderEntity = PetStuff.Order.Domain.Entities.Order;
using OrderItemEntity = PetStuff.Order.Domain.Entities.OrderItem;

namespace PetStuff.Order.Infrastructure.Context
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<OrderEntity> Orders => Set<OrderEntity>();
        public DbSet<OrderItemEntity> OrderItems => Set<OrderItemEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<OrderEntity>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderEntity>()
                .Property(o => o.TotalPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderItemEntity>()
                .Property(i => i.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderEntity>()
                .Property(o => o.ShippingAddress)
                .IsRequired()
                .HasMaxLength(500);

            modelBuilder.Entity<OrderItemEntity>()
                .Property(i => i.ProductName)
                .IsRequired()
                .HasMaxLength(200);
        }
    }
}
