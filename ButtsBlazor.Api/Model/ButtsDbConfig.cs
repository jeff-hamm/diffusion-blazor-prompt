using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ButtsBlazor.Api.Model
{
    public static class ButtsDbConfig
    {
        public static IServiceCollection AddButtsDb(this IServiceCollection services)
        {

            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = System.IO.Path.Join(path, "butts.db");
            services.AddDbContext<ButtsDbContext>(op => op.UseSqlite($"Data Source=butts.db"), optionsLifetime: ServiceLifetime.Singleton)
                .AddDbContextFactory<ButtsDbContext>(b => b.UseSqlite($"Data Source=butts.db"));
            return services;
        }


        public static string? DbPath { get; private set; }
    }
}
