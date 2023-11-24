using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace POS.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Products> Products { get; set; }
        public DbSet<Employees> Employees { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<Ingredients> Ingredients { get; set; }
        public DbSet<Payments> Payments { get; set; }
        public DbSet<RecipeIngredients> RecipeIngredients { get; set; }
        public DbSet<Recipes> Recipes { get; set; }
        public DbSet<ToDoListTask> ToDoListTasks { get; set; }

        public static string DatabasePath { get; private set; }

        static AppDbContext()
        {
            string databaseLocation = @"..\..\..\Database\barmanagement.db";
            string projectPath = Directory.GetCurrentDirectory();
            string absolutePath = Path.Combine(projectPath, databaseLocation);

            DatabasePath = $"Data Source=" + absolutePath;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(DatabasePath);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employees>().HasKey(e => e.Employee_id);
            modelBuilder.Entity<Products>().HasKey(e => e.Product_id);
            modelBuilder.Entity<Orders>().HasKey(e => e.Order_id);
            modelBuilder.Entity<Ingredients>().HasKey(e => e.Ingredient_id);
            modelBuilder.Entity<Payments>().HasKey(e => e.Payment_id);
            modelBuilder.Entity<RecipeIngredients>().HasKey(e => e.RecipeIngredient_id);
            modelBuilder.Entity<Recipes>().HasKey(e => e.Recipe_id);
            modelBuilder.Entity<ToDoListTask>().HasKey(e => e.TodoTask_Id);
        }
    }
}
