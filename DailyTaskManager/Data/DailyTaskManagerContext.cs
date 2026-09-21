using System;
using System.Collections.Generic;
using DailyTaskManager.Models;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;


namespace DailyTaskManager.Data;

public partial class DailyTaskManagerContext : DbContext
{
    public DailyTaskManagerContext()
    {
    }

    public DailyTaskManagerContext(DbContextOptions<DailyTaskManagerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Models.Task> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Models.Task>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.Property(e => e.CreateDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Priority).HasDefaultValueSql("'Medium'");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}