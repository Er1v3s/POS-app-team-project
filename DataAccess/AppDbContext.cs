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

            optionsBuilder.UseSqlite(ConnectionString, builder => builder.MigrationsAssembly("DbSeeder"));
        }

        public DbSet<Product> Product { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<ToDoListTask> ToDoListTasks { get; set; }
        public DbSet<EmployeeWorkSession> EmployeeWorkSession { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employees").HasKey(e => e.EmployeeId);
            modelBuilder.Entity<Product>().ToTable("Products").HasKey(e => e.ProductId);
            modelBuilder.Entity<Order>().ToTable("Orders").HasKey(e => e.OrderId);
            modelBuilder.Entity<Ingredient>().ToTable("Ingredients").HasKey(e => e.IngredientId);
            modelBuilder.Entity<Payment>().ToTable("Payments").HasKey(e => e.PaymentId);
            modelBuilder.Entity<RecipeIngredient>().ToTable("RecipeIngredients").HasKey(e => e.RecipeIngredientId);
            modelBuilder.Entity<Recipe>().ToTable("Recipes").HasKey(e => e.RecipeId);
            modelBuilder.Entity<ToDoListTask>().HasKey(e => e.TodoTaskId);
            modelBuilder.Entity<OrderItem>().ToTable("OrderItems").HasKey(e => e.OrderItemId);
            modelBuilder.Entity<EmployeeWorkSession>().HasKey(e => e.WorkSessionId);
        }
    }
}