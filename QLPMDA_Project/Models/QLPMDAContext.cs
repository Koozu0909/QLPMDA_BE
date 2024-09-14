using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace QLPMDA_Project.Models;

public partial class QLPMDAContext : DbContext
{
    public QLPMDAContext(DbContextOptions<QLPMDAContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Categories> Categories { get; set; }

    public virtual DbSet<Products> Products { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categories>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC078799963A");

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.CategoryName).HasMaxLength(250);
            entity.Property(e => e.Description).HasMaxLength(1000);
        });

        modelBuilder.Entity<Products>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Products__3214EC07D4B6B026");

            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.Property(e => e.Price).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.ProductName).HasMaxLength(250);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("FK__Products__Update__3F466844");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Active).HasDefaultValue(true);
            entity.Property(e => e.Avatar).HasMaxLength(1000);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FullName).HasMaxLength(250);
            entity.Property(e => e.Password).HasMaxLength(100);
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Salt)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
