using Microsoft.AspNetCore.Authentication;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ServerlessPDFConversionDemo.Startup))]
namespace ServerlessPDFConversionDemo
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddOptions<AuthenticationOptions>().Configure<IConfiguration>((setttings, configuration) => 
            {
                configuration.GetSection("graph").Bind(setttings);
            });
            builder.Services.AddOptions<PdfOptions>().Configure<IConfiguration>((setttings, configuration) =>
            {
                configuration.GetSection("pdf").Bind(setttings);
            });

            builder.Services.AddSingleton<AuthenticationService>();
            builder.Services.AddSingleton<FileService>();
        }
    }
}
