using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FinancialTracker.Infrastructure.Persistence.Data;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<ProcessedMessage> ProcessedMessages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=FinacialAppDB;Trusted_Connection=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC07A8037261");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Amount).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Customer).HasMaxLength(255);
            entity.Property(e => e.Status).HasMaxLength(50);
        });

        modelBuilder.Entity<ProcessedMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Processe__3214EC07DA4B5BD8");

            entity.Property(e => e.MessageKey).HasMaxLength(255);
            entity.Property(e => e.ProcessedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Topic).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
