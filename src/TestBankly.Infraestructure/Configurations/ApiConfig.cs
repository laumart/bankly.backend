using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TestBankly.Infraestructure.Data;

namespace TestBankly.Infraestructure.Configurations
{
    public static class ApiConfig
    {
        //public static void AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddScoped<TransactionContext>();

        //    services.AddDbContext<TransactionContext>(options =>
        //            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            
        //}
    }
}