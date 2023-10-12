using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scradic.Core.Interfaces;
using Scradic.Core.Interfaces.Repositories;
using Scradic.Core.Interfaces.Services;
using Scradic.Infrastructure.Data;
using Scradic.Infrastructure.Repositories;
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
                services.AddSingleton<IUserRepository, UserRepository>();
                services.AddSingleton<IPDFRepository, PDFRepository>();

                //Services
                services.AddSingleton<IStart, Start>();
                services.AddSingleton<IUserService, UserService>();
                services.AddSingleton<IWordService, WordService>();
                services.AddSingleton<IPDFService, PDFService>();

                //Email Sender
                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();

                services.AddTransient<IEmailService, EmailService>();

                services.AddMemoryCache();
            }).Build();

            var app = _host.Services.GetRequiredService<IStart>();
            app.StartScradic();
        }
    }
}