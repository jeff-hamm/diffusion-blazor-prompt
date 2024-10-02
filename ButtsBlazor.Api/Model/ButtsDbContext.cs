using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ButtsBlazor.Api.Utils;
using ButtsBlazor.Services;
using System.Reflection.Emit;
using Configuration.EFCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ButtsBlazor.Api.Model;

public class ButtsDbContext: DbContext
{
    public virtual DbSet<SettingEntity> Settings { get; set; }
    public virtual DbSet<PromptEntity> Prompts { get; set; } = null!;
    public virtual DbSet<PromptArgs> PromptArgs { get; set; } = null!;
    public virtual DbSet<ImageEntity> Images { get; set; } = null!;

    public ButtsDbContext(DbContextOptions<ButtsDbContext> options) : base(options)
    {
    }

    public ButtsDbContext()
    {
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder
            .Properties<WebPath>()
            .HaveConversion<WebPathConverter, WebPathComparer>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.BuildSettingEntityModel();
        modelBuilder.Entity<ImageMetadata>().HasKey(x => x.RowId);
        modelBuilder.Entity<ImageEntity>().Property(x => x.RowId).ValueGeneratedOnAdd();
        modelBuilder.Entity<ImageMetadata>().HasKey(x => x.RowId);
        modelBuilder.Entity<ImageMetadata>().Property(x => x.RowId).ValueGeneratedOnAdd();
        modelBuilder.Entity<PromptArgs>().HasKey(x => x.RowId);
        modelBuilder.Entity<PromptArgs>().Property(x => x.RowId).ValueGeneratedOnAdd();
        modelBuilder.Entity<PromptEntity>().HasKey(x => x.RowId);
        modelBuilder.Entity<PromptEntity>().Property(x => x.RowId).ValueGeneratedOnAdd();
        modelBuilder.Entity<ImageEntity>().HasIndex(e => e.Type);
        modelBuilder.Entity<ImageEntity>().HasIndex(e => e.Path).IsUnique();
        base.OnModelCreating(modelBuilder);
    }
    public const string DefaultDbPath = "db/butts.db";

    public static readonly string DefaultConnectionString = $"Data Source={DefaultDbPath}";
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite(DefaultConnectionString);
        base.OnConfiguring(optionsBuilder);
    }
}
public class WebPathConverter : ValueConverter<WebPath, string>
{
    public WebPathConverter()
        : base(
            v => v.Path,
            v => new WebPath(v))
    {
    }
}

public class WebPathComparer : ValueComparer<WebPath>
{
    public WebPathComparer()
        : base(
            (v1, v2) => v1.Equals(v2),
            v => v.GetHashCode())
    {
    }
}