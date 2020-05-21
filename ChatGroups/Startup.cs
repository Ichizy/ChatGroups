using ChatGroups.Data;
using ChatGroups.HubProcessors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ChatGroups.Data.Repositories;
using ChatGroups.Services;
using Serilog;

namespace ChatGroups
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            //TBD: Currently EntityFrameworkCore + InMemory storage used. For real project, DB would be used instead
            services.AddDbContext<StorageContext>(opt => opt.UseInMemoryDatabase("ChatGroups"));

            //TBD: I would prefer to use Autofac and extract these to a separate module.
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IProcessor, Processor>();

            services.AddCors();
            services.AddSignalR();

            services.AddOptions<AppConfiguration>()
                            .Bind(Configuration.GetSection(nameof(AppConfiguration)))
                            .ValidateDataAnnotations();

            var appConfig = Configuration.GetSection(nameof(AppConfiguration)).Get<AppConfiguration>();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo
                .ApplicationInsights(appConfig.InstrumentationKey, TelemetryConverter.Events)
                .CreateLogger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chat",
                    options =>
                    {
                        options.Transports = HttpTransportType.WebSockets;
                    });


                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Welcome to Next Games!");
                });
            });
        }
    }
}
