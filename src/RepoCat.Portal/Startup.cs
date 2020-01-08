﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.AspNetCore.Extensions;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RepoCat.Persistence.Service;
using RepoCat.Portal.Mapping;
using RepoCat.Portal.RecurringJobs;
using RepoCat.RepositoryManagement.Service;
using RepoCat.Transmission.Client;

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
            
            services.Configure<RepositoryMonitoringSettings>(this.Configuration.GetSection("RepositoryMonitoringSettings"));
            services.AddSingleton<IRepositoryMonitoringSettings>(sp => sp.GetRequiredService<IOptions<RepositoryMonitoringSettings>>().Value);




            services.AddSingleton<RepositoryDatabase>();
            services.AddScoped<IRepositoryManagementService, RepositoryManagementService>();
            AddRecurringRepositoryScanJob(services);
            ConfigureAutoMapper(services);
            this.AddHangfire(services);

            services.AddMvc().AddNewtonsoftJson().AddRazorRuntimeCompilation();
            AddApplicationInsights(services);
        }

        private static void AddRecurringRepositoryScanJob(IServiceCollection services)
        {
            services.AddScoped<ISender, DirectProjectInfoImporter>();
            services.AddScoped<ILogger>(x=> new TraceLogger(LogLevel.Info));
            services.AddScoped<ITransmissionClient, TransmissionClient>();
            services.AddScoped<IScanRepositoryJob, ScanRepositoryJob>();
        }

        private static void AddApplicationInsights(IServiceCollection services)
        {
            ApplicationInsightsServiceOptions aiOptions = new ApplicationInsightsServiceOptions
            {
                DeveloperMode = false
            };
            TelemetryDebugWriter.IsTracingDisabled = true;

            services.AddApplicationInsightsTelemetry(aiOptions);
        }


        private static void ConfigureAutoMapper(IServiceCollection services)
        {
            var mappingConfig = MappingConfigurationFactory.Create();
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }

        private void AddHangfire(IServiceCollection services)
        {
            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(this.Configuration.GetConnectionString("HangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true,
                }));

            services.AddHangfireServer();

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
            this.UseHangfire(app);

            app.UseRouting();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapAreaControllerRoute(
                    name: "Catalog",
                    areaName: "Catalog",
                    pattern: "{area=Catalog}/{controller=Search}/{action=Index}/{id?}");
              
            });

        }

        private void UseHangfire(IApplicationBuilder app)
        {
            app.UseHangfireDashboard();
            app.UseHangfireServer();
            var settings1 = this.Configuration.GetSection("RepositoryMonitoringSettings");
            var settings2 = this.Configuration.Get<RepositoryMonitoringSettings>();
            var settings3 = this.Configuration.GetSection("RepositoryMonitoringSettings").Get<List<RepositoryToScanSettings>>();
            var settings = this.Configuration.GetSection("RepositoryMonitoringSettings").Get<RepositoryMonitoringSettings>();
            var telemetry = app.ApplicationServices.GetService<TelemetryClient>();
            RepoCatRecurringJobsScheduler.ScheduleRecurringJobs(settings, telemetry);
        }
    }
}
