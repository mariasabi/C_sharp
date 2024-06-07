using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Order_management.Models;

public partial class OrderManagementContext : DbContext
{
    public OrderManagementContext()
    {
    }

    public OrderManagementContext(DbContextOptions<OrderManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Item> Items { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //       => optionsBuilder.UseSqlServer("Server=DESKTOP-OG64OM8\\SQLEXPRESS;Database=OrderManagement;User Id=DESKTOP-OG64OM8\\P10;Trusted_Connection=True;TrustServerCertificate=True;");
    { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Item>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.ToTable("Item");

            entity.Property(e => e.Id)
            .HasColumnName("Id").ValueGeneratedOnAdd();
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("Name");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
            .HasColumnName("Type");
            entity.Property(e => e.Quantity).
            HasColumnName("Quantity");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
