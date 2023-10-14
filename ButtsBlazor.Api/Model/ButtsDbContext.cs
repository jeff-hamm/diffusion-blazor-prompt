using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using ButtsBlazor.Api.Utils;
using ButtsBlazor.Services;

namespace ButtsBlazor.Api.Model;

public class ButtsDbContext: DbContext
{
    public virtual DbSet<PromptEntity> Prompts { get; set; } = null!;
    public virtual DbSet<PromptArgs> PromptArgs { get; set; } = null!;
    public virtual DbSet<ImageEntity> Images { get; set; } = null!;
    public ButtsDbContext(DbContextOptions<ButtsDbContext> options) : base(options)
    {
    }

    public ButtsDbContext()
    {
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
//        if (optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlite($"Data Source=butts.db");
        base.OnConfiguring(optionsBuilder);
    }
}