﻿using System;
using System.Text;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;
using PodNoms.Api.Persistence;
using PodNoms.Api.Providers;
using PodNoms.Api.Services;
using PodNoms.Api.Services.Auth;
using PodNoms.Api.Services.Processor.Hangfire;
using Microsoft.AspNetCore.Mvc.Formatters;
using PodNoms.Api.Services.Storage;
using Microsoft.AspNetCore.Http.Features;
using PodNoms.Api.Services.Realtime;

namespace PodNoms.Api
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddDbContext<PodnomsDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddOptions();
            services.Configure<AppSettings>(Configuration.GetSection("App"));
            services.Configure<StorageSettings>(Configuration.GetSection("Storage"));
            services.Configure<ImageFileStorageSettings>(Configuration.GetSection("ImageFileStorageSettings"));
            services.Configure<AudioFileStorageSettings>(Configuration.GetSection("AudioFileStorageSettings"));

            services.AddAutoMapper(e =>
            {
                e.AddProfile(new MappingProvider(Configuration));
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            services.AddJwtBearerAuthentication(options =>
            {
                /* configure options.TokenValidationParameters */
                options.Audience = Configuration["auth0:clientId"];
                options.Authority = $"https://{Configuration["auth0:domain"]}/";
                options.Events = new JwtBearerEvents()
                {
                    OnTokenValidated = AuthenticationMiddleware.OnTokenValidated
                };
            });
            var defaultPolicy =
                new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes("Bearer")
                .RequireAuthenticatedUser()
                .Build();

            services.AddAuthorization(j =>
            {
                j.DefaultPolicy = defaultPolicy;
            });

            services.AddMvc(options =>
            {
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            }).AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;

            }).AddXmlSerializerFormatters();

            services.Configure<FormOptions>(x =>
            {
                x.ValueLengthLimit = int.MaxValue;
                x.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(Configuration["ConnectionStrings:DefaultConnection"]);
                config.UseColouredConsoleLogProvider();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            services.AddTransient<IFileUploader, AzureFileUploader>();
            services.AddTransient<IFileStorage, AzureFileStorage>();
            services.AddTransient<IRealTimeUpdater, PusherUpdater>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPodcastRepository, PodcastRepository>();
            services.AddScoped<IEntryRepository, EntryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUrlProcessService, UrlProcessService>();

            //register the codepages (required for slugify)
            var instance = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(instance);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env,
            ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            // Console.WriteLine("Performing migrations");
            using (var context = new PodnomsDbContext(
                app.ApplicationServices.GetRequiredService<DbContextOptions<PodnomsDbContext>>()))
            {
                context.Database.Migrate();
            }
            Console.WriteLine("Successfully migrated");

            app.UseStaticFiles();

            GlobalConfiguration.Configuration.UseActivator(new ServiceProviderActivator(serviceProvider));

            if (env.IsProduction() || true)
            {
                app.UseHangfireServer();
                app.UseHangfireDashboard();
            }
            app.UseCors("AllowAllOrigins");
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
