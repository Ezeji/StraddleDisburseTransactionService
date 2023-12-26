using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using StraddleDisburseTransactionApi.Middleware;
using StraddleDisburseTransactionData;
using StraddleDisburseTransactionCore.Models;
using StraddleDisburseTransactionRepository;
using StraddleDisburseTransactionCore.Services;
using Microsoft.Extensions.DependencyInjection;
using StraddleDisburseTransactionCore.BackgroundServices;
using StraddleDisburseTransactionCore.Configurations;
using StraddleDisburseTransactionCore.Configurations.Azure.Interfaces;

namespace StraddleDisburseTransactionApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //we keep using NewtonSoft so that serialization of reference loop can be ignored, especially because of EFCore
            services.AddControllers()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore)
                .AddNewtonsoftJson(x => x.SerializerSettings.Converters.Add(new StringEnumConverter()));

            services.AddDbContext<StraddleDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("StraddleDb"), b => b.MigrationsAssembly("StraddleDisburseTransactionData"));
            });

            services.AddCors(option =>
            {
                option.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddHttpClient();
            services.AddHttpContextAccessor();
            
            //Add external configurations
            services.AddConfigurations();

            //Add custom Core Repo and Service
            services.AddCoreRepository();
            services.AddServices();

            //Add our configs
            services.AddConfigSettings(Configuration);

            //Add background services
            services.AddHostedService<DisburseTransactionBackgroundService>();
            services.AddHostedService<AzureServiceBusBackgroundService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Straddle Disburse Transaction Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseMiddleware<CustomErrorHandlerMiddleware>(); //custom error handler
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseCors();

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseSwagger();
                app.UseSwaggerUI(opt =>
                {
                    opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Straddle Disburse Transaction Service");
                });
            }

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

    }
}
