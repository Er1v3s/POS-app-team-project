using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AppDbContext : DbContext
    {
        private static readonly string ConnectionString;

        static AppDbContext()
        {
            ConnectionString = new DatabaseConfiguration().GetConnectionString();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Production 
            //optionsBuilder.UseSqlServer(ConnectionString);

            // Development
            optionsBuilder.UseSqlite(ConnectionString, b => b.MigrationsAssembly("DbSeeder"));
        }

        public DbSet<Product> Product { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<Ingredients> Ingredients { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<RecipeIngredients> RecipeIngredients { get; set; }
        public DbSet<Recipes> Recipes { get; set; }
        public DbSet<ToDoListTask> ToDoListTasks { get; set; }
        public DbSet<EmployeeWorkSession> EmployeeWorkSession { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employees>().HasKey(e => e.EmployeeId);
            modelBuilder.Entity<Product>().ToTable("Products").HasKey(e => e.ProductId);
            modelBuilder.Entity<Orders>().HasKey(e => e.OrderId);
            modelBuilder.Entity<Ingredients>().HasKey(e => e.IngredientId);
            modelBuilder.Entity<Payments>().HasKey(e => e.PaymentId);
            modelBuilder.Entity<RecipeIngredients>().HasKey(e => e.RecipeIngredientId);
            modelBuilder.Entity<Recipes>().HasKey(e => e.RecipeId);
            modelBuilder.Entity<ToDoListTask>().HasKey(e => e.TodoTaskId);
            modelBuilder.Entity<OrderItems>().HasKey(e => e.OrderItemId);
            modelBuilder.Entity<EmployeeWorkSession>().HasKey(e => e.WorkSessionId);
        }
    }
}
