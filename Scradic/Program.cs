using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scradic.Core.Interfaces;
using Scradic.Infrastructure.Data;
using Scradic.Infrastructure.Repositories;
using Scradic.Interfaces;
using Scradic.Services;

namespace Scradic
{
    class Program
    {
        public static void Main(string[] args)
        {
            IHost _host = Host.CreateDefaultBuilder().ConfigureServices(services =>
            {
                //DbContext
                services.AddDbContext<AppDbContext>();

                //Repositories
                services.AddSingleton<IWordRepository, WordRepository>();

                //Services
                services.AddSingleton<IWordService, WordService>();
                services.AddSingleton<IStart, Start>();

                services.AddMemoryCache();
            }).Build();

            var app = _host.Services.GetRequiredService<IStart>();
            app.StartScradic();
        }
    }
}