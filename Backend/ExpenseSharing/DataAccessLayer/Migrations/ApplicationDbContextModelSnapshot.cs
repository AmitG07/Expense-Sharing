﻿// <auto-generated />
using System;
using DataAccessLayer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace DataAccessLayer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.31")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BusinessObjectLayer.Expense", b =>
                {
                    b.Property<int>("ExpenseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ExpenseId"), 1L, 1);

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("ExpenseAmount")
                        .HasColumnType("float");

                    b.Property<DateTime>("ExpenseCreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<bool>("IsSettled")
                        .HasColumnType("bit");

                    b.Property<int>("PaidByUserId")
                        .HasColumnType("int");

                    b.HasKey("ExpenseId");

                    b.HasIndex("GroupId");

                    b.HasIndex("PaidByUserId");

                    b.ToTable("Expenses");
                });

            modelBuilder.Entity("BusinessObjectLayer.ExpenseSplit", b =>
                {
                    b.Property<int>("ExpenseSplitId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ExpenseSplitId"), 1L, 1);

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<int>("ExpenseId")
                        .HasColumnType("int");

                    b.Property<double>("SplitAmount")
                        .HasColumnType("float");

                    b.Property<int>("SplitWithUserId")
                        .HasColumnType("int");

                    b.HasKey("ExpenseSplitId");

                    b.HasIndex("ExpenseId");

                    b.HasIndex("SplitWithUserId");

                    b.ToTable("ExpenseSplits");
                });

            modelBuilder.Entity("BusinessObjectLayer.Group", b =>
                {
                    b.Property<int>("GroupId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GroupId"), 1L, 1);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<int>("GroupAdminId")
                        .HasColumnType("int");

                    b.Property<string>("GroupName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<double>("TotalExpense")
                        .HasColumnType("float");

                    b.Property<int>("TotalMembers")
                        .HasColumnType("int");

                    b.HasKey("GroupId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("BusinessObjectLayer.GroupMember", b =>
                {
                    b.Property<int>("GroupMemberId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("GroupMemberId"), 1L, 1);

                    b.Property<double>("GivenAmount")
                        .HasColumnType("float");

                    b.Property<int>("GroupId")
                        .HasColumnType("int");

                    b.Property<double>("TakenAmount")
                        .HasColumnType("float");

                    b.Property<double>("UserExpense")
                        .HasColumnType("float");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("GroupMemberId");

                    b.HasIndex("GroupId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupMembers");
                });

            modelBuilder.Entity("BusinessObjectLayer.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"), 1L, 1);

                    b.Property<decimal>("AvailableBalance")
                        .HasColumnType("decimal(18,2)");

                    b.Property<string>("EmailId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("BusinessObjectLayer.Expense", b =>
                {
                    b.HasOne("BusinessObjectLayer.Group", "Group")
                        .WithMany("Expense")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObjectLayer.User", "User")
                        .WithMany()
                        .HasForeignKey("PaidByUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObjectLayer.ExpenseSplit", b =>
                {
                    b.HasOne("BusinessObjectLayer.Expense", "Expense")
                        .WithMany("ExpenseSplit")
                        .HasForeignKey("ExpenseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObjectLayer.User", "User")
                        .WithMany()
                        .HasForeignKey("SplitWithUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Expense");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObjectLayer.GroupMember", b =>
                {
                    b.HasOne("BusinessObjectLayer.Group", "Group")
                        .WithMany("GroupMember")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BusinessObjectLayer.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BusinessObjectLayer.Expense", b =>
                {
                    b.Navigation("ExpenseSplit");
                });

            modelBuilder.Entity("BusinessObjectLayer.Group", b =>
                {
                    b.Navigation("Expense");

                    b.Navigation("GroupMember");
                });
#pragma warning restore 612, 618
        }
    }
}