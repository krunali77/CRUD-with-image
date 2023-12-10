using Cars.Models;
using Microsoft.EntityFrameworkCore;

namespace Cars.Data
{
    public class CarDbContext: DbContext
    {
        public CarDbContext(DbContextOptions<CarDbContext> options) :base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Car>(entity =>
            {
                entity.HasKey(x => x.Id);

            });
        }

        public DbSet<Car> CarModels { get; set; }
    }
}
