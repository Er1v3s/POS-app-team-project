using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models
{
    public class AppDbContext : DbContext
    {
        // Azure production database
        //private static readonly string ConnectionString = "Server=tcp:pos-app-team-project.database.windows.net,1433;Initial Catalog=barmanagementdb;Persist Security Info=False;User ID=DatabaseUser;Password=!Q@w#E123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(ConnectionString);
        //}


        // Local dev database
        public static string DatabasePath { get; private set; }

        static AppDbContext()
        {
            //string databaseLocation = @"..\..\..\Database\barmanagement.db";
            //string databaseLocation = "C:\\Users\\filip\\Programing\\C#\\DbSeeder\\DbSeeder\\Database\\barmanagement.db";
            string databaseLocation = @"C:\Users\filip\Programing\C#\POS-app\POS-app-team-project\POS\Database\barmanagement.db";
            string projectPath = Directory.GetCurrentDirectory();
            string absolutePath = Path.Combine(projectPath, databaseLocation);

            DatabasePath = $"Data Source=" + absolutePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(DatabasePath, b => b.MigrationsAssembly("DbSeeder"));
        }

        public DbSet<Products> Products { get; set; }
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
            modelBuilder.Entity<Products>().HasKey(e => e.ProductId);
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
