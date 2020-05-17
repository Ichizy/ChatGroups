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
            services.AddDbContext<StorageContext>(opt => opt.UseInMemoryDatabase("ChatGroups"));

            //TODO: extract this to a separate module (if possible in default DI)
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IMessageRepository, MessageRepository>();

            services.AddCors();
            services.AddSignalR();
            //TODO: add proper implementation for appConfig
            services.Configure<AppConfiguration>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Make sure the CORS middleware is ahead of SignalR.
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<GroupsHub>("/chat",
                    options =>
                    {
                        options.Transports = HttpTransportType.WebSockets;
                    });


                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
