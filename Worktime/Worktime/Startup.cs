using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Worktime.Models;
using Worktime.Services;
using Worktime.Settings;

namespace Worktime
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();

            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            services.AddDbContext<WorktimeDbContext>();
            services.AddSingleton<IMailService, MailService>(); // Register IMailService and its implementation
            
            services.Configure<MailSettings>(config.GetSection("MailSettings"));

            services.AddSingleton<ScheduleService>();
            services.AddHostedService<ScheduleService>();
        }
    }
}
