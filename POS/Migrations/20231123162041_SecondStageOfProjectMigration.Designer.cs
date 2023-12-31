﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using POS.Models;

#nullable disable

namespace POS.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20231123162041_SecondStageOfProjectMigration")]
    partial class SecondStageOfProjectMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.13");

            modelBuilder.Entity("POS.Models.Employees", b =>
                {
                    b.Property<int>("Employee_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Address")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("First_name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("Hire_date")
                        .HasColumnType("TEXT");

                    b.Property<string>("Job_title")
                        .HasColumnType("TEXT");

                    b.Property<string>("Last_name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("Phone_number")
                        .HasColumnType("INTEGER");

                    b.HasKey("Employee_id");

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("POS.Models.Ingredients", b =>
                {
                    b.Property<int>("Ingredient_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Unit")
                        .HasColumnType("TEXT");

                    b.HasKey("Ingredient_id");

                    b.ToTable("Ingredients");
                });

            modelBuilder.Entity("POS.Models.Orders", b =>
                {
                    b.Property<int>("Order_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Employee_id")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Orider_time")
                        .HasColumnType("TEXT");

                    b.Property<int>("Product_id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.HasKey("Order_id");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("POS.Models.Payments", b =>
                {
                    b.Property<int>("Payment_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<double>("Amount")
                        .HasColumnType("REAL");

                    b.Property<int>("Order_id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Payment_method")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("Payment_time")
                        .HasColumnType("TEXT");

                    b.HasKey("Payment_id");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("POS.Models.Products", b =>
                {
                    b.Property<int>("Product_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Category")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<bool?>("IsAvailable")
                        .HasColumnType("INTEGER");

                    b.Property<double?>("Price")
                        .HasColumnType("REAL");

                    b.Property<string>("Product_name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Recipe_id")
                        .HasColumnType("INTEGER");

                    b.HasKey("Product_id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("POS.Models.RecipeIngredients", b =>
                {
                    b.Property<int>("RecipeIngredient_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("Ingredient_id")
                        .HasColumnType("INTEGER");

                    b.Property<double>("Quantity")
                        .HasColumnType("REAL");

                    b.Property<int>("Recipe_id")
                        .HasColumnType("INTEGER");

                    b.HasKey("RecipeIngredient_id");

                    b.ToTable("RecipeIngredients");
                });

            modelBuilder.Entity("POS.Models.Recipes", b =>
                {
                    b.Property<int>("Recipe_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Recipe")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Recipe_name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Recipe_id");

                    b.ToTable("Recipes");
                });
#pragma warning restore 612, 618
        }
    }
}
