using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Mapping;
using RepoCat.RepositoryManagement.Service;

#pragma warning disable 1591

namespace RepoCat.Portal
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.Configure<RepoCatDbSettings>(this.Configuration.GetSection("RepoCatDbSettings"));

            services.AddSingleton<IRepoCatDbSettings>(sp => sp.GetRequiredService<IOptions<RepoCatDbSettings>>().Value);
            services.AddSingleton<RepositoryDatabase>();
            services.AddScoped<IRepositoryManagementService, RepositoryManagementService>();

            ConfigureAutoMapper(services);

            services.AddControllersWithViews();
            services.AddApplicationInsightsTelemetry();

        }

        private static void ConfigureAutoMapper(IServiceCollection services)
        {
            var mappingConfig = MappingConfigurationFactory.Create();
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
#pragma warning disable CA1822 // Mark members as static
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
#pragma warning restore CA1822 // Mark members as static
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "Catalog",
                    pattern: "{area=Catalog}/{controller=Search}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action}/{id?}");
            });
        }

        
    }
}
