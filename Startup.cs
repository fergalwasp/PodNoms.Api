using System;
using System.Text;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PodNoms.Api.Models;
using PodNoms.Api.Models.ViewModels;
using PodNoms.Api.Services;
using PodNoms.Api.Services.Auth;
using PodNoms.Api.Services.Processor.Hangfire;

namespace PodNoms.Api {
    public class Startup {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services) {
            services.AddDbContext<PodnomsContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Podnoms")));

            services.AddOptions();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            services.AddAuthentication(o => {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });
            
            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            });

            services.AddHangfire(config => {
                config.UseSqlServerStorage(Configuration.GetConnectionString("Podnoms"));
            });

            services.AddCors(options => {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            //register automapper
            var mapperConfiguration = new AutoMapper.MapperConfiguration(cfg => {
                cfg.CreateMap<PodcastViewModel, Podcast>();
                cfg.CreateMap<PodcastEntryViewModel, PodcastEntry>();
            });

            var mapper = mapperConfiguration.CreateMapper();

            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.AddScoped<IPodcastRepository, PodcastRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IMapper>(sp => mapperConfiguration.CreateMapper());
            services.AddSingleton<IUrlProcessService, UrlProcessService>();

            //register the codepages (required for slugify)
            var instance = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(instance);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider serviceProvider) {

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            var options = new JwtBearerOptions {
                Audience = Configuration["auth0:clientId"],
                    Authority = $"https://{Configuration["auth0:domain"]}/",
                    Events = new JwtBearerEvents() {
                        OnTokenValidated = AuthenticationMiddleware.OnTokenValidated
                    }
            };

            GlobalConfiguration.Configuration.UseActivator(new ServiceProviderActivator(serviceProvider));
            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.UseCors("AllowAllOrigins");
            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}