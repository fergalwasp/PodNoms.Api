using System;
using System.Text;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
using PodNoms.Api.Utils.Azure;
using Microsoft.AspNetCore.Mvc.Formatters;

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

            Console.WriteLine(Configuration["ConnectionStrings:Default"]);
            
            services.AddDbContext<PodnomsDbContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:Default"]));

            services.AddOptions();
            services.Configure<AppSettings>(Configuration.GetSection("App"));
            services.Configure<AudioStorageSettings>(Configuration.GetSection("AudioStorage"));
            services.Configure<ImageSettings>(Configuration.GetSection("PhotoSettings"));

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

            services.AddMvc(options => {
                options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
            }).AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
               
            }).AddXmlSerializerFormatters();

            services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(Configuration["ConnectionStrings:Default"]);
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

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<IFileUploader, FileUploader>();
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

            Console.WriteLine("Performing migrations");
            using (var context = new PodnomsDbContext(
                app.ApplicationServices.GetRequiredService<DbContextOptions<PodnomsDbContext>>()))
            {
                context.Database.Migrate();
            }
            Console.WriteLine("Successfully migrated");

            app.UseStaticFiles();

            GlobalConfiguration.Configuration.UseActivator(new ServiceProviderActivator(serviceProvider));

            if (true) //env.IsProduction())
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
