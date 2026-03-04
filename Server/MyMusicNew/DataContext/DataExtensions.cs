using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces; // חשוב כדי לזהות את IContext
using DataContext;

namespace DataContext
{
    public static class DataExtensions
    {
        public static IServiceCollection AddDataLayer(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<MusicContext>(options =>
                options.UseSqlServer(connectionString,
                b => b.MigrationsAssembly("DataContext")));
            services.AddScoped<IContext, MusicContext>();

            return services;
        }
    }
}