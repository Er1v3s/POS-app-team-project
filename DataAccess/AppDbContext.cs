﻿using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Product { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<Recipe> Recipe { get; set; }
        public DbSet<ToDoListTask> ToDoListTasks { get; set; }
        public DbSet<EmployeeWorkSession> EmployeeWorkSession { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().ToTable("Employees").HasKey(e => e.EmployeeId);

            modelBuilder.Entity<Product>()
                .ToTable("Products")
                .HasOne(p => p.Recipe)
                .WithOne(r => r.Product)
                .HasForeignKey<Product>(p => p.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Order>().ToTable("Orders").HasKey(e => e.OrderId);

            modelBuilder.Entity<Ingredient>().ToTable("Ingredients")
                .HasMany<RecipeIngredient>()
                .WithOne(ri => ri.Ingredient)
                .HasForeignKey(ri => ri.IngredientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>().ToTable("Payments").HasKey(e => e.PaymentId);
            modelBuilder.Entity<RecipeIngredient>().ToTable("RecipeIngredients").HasKey(e => e.RecipeIngredientId);

            modelBuilder.Entity<Recipe>().ToTable("Recipes")
                .HasMany(r => r.RecipeIngredients)
                .WithOne(ri => ri.Recipe)
                .HasForeignKey(ri => ri.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ToDoListTask>().HasKey(e => e.TodoTaskId);
            modelBuilder.Entity<OrderItem>().ToTable("OrderItems").HasKey(e => e.OrderItemId);
            modelBuilder.Entity<EmployeeWorkSession>().HasKey(e => e.WorkSessionId);
        }
    }
}