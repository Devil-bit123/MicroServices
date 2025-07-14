using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ProductsMicroService.Models;

public partial class ProductsCatalogContext : DbContext
{
    public ProductsCatalogContext()
    {
    }

    public ProductsCatalogContext(DbContextOptions<ProductsCatalogContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Product> Products { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Description)
                .HasMaxLength(254)
                .IsUnicode(false);
            entity.Property(e => e.IdProduct).ValueGeneratedOnAdd();
            entity.Property(e => e.Price).HasColumnType("decimal(18, 6)");
            entity.Property(e => e.ProductName)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.Sku)
                .HasMaxLength(128)
                .IsUnicode(false)
                .HasColumnName("SKU");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
