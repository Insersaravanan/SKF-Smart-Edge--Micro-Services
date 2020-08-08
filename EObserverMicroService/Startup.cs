using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using EMaintanance.Utils;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EMaintanance.UserModels;
using EMaintananceApi.UserModels;
using EMaintanance.Providers;
using Swashbuckle.AspNetCore.Swagger;
using EObserver.Providers;
using Swashbuckle.AspNetCore.SwaggerUI;
using Quartz.Spi;
using Quartz;
using EMaintanenceMicroService.Services.Schedule;
using EMaintanenceMicroService.Services;
using Quartz.Impl;
using System.Net.Http;

namespace EMaintanance
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public static string MasterConnectionString
        {
            get;
            private set;
        }

        public static string UserConnectionString
        {
            get;
            private set;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //EObserver Customized authorization
            services.AddSingleton<IAuthorizationPolicyProvider, EObserverPolicyProvider>();
            services.AddTransient<IAuthorizationHandler, EObserverAuthorizationHandler>();
            services.AddHttpContextAccessor();

            //Adding Cors Support
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .AllowCredentials();
                });
            });

            //services.AddTransient<IJobFactory, JobFactory>(
            //    (provider) =>
            //    {
            //        return new JobFactory(provider);
            //    });
            // Add Quartz services
            services.AddSingleton<IJobFactory, JobFactory>();
            services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.AddSingleton<QuartzJobRunner>();
            services.AddHostedService<QuartzService>();

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
            })
           .AddEntityFrameworkStores<UserDbContext>()
           .AddDefaultTokenProviders();

            services.AddDbContext<UserDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("IFin.User")));

            // return 401 instead of redirect to login
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    context.Response.Headers["Location"] = context.RedirectUri;
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

            //services.AddDefaultIdentity<IdentityUser>()
            //   .AddEntityFrameworkStores<ApplicationDbContext>();

            //services.AddScoped<ApplicationSignInManager>();

            services.AddMvc(options =>
                options.AllowCombiningAuthorizeFilters = false //For IFin Customized autorization
            ).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Smart Edge Gen2 & SKF-Emaintenance Data Service",
                    Description = "Smart Edge-Emaintenance Data Service Documentation",
                    TermsOfService = "We are Building...",
                    Contact = new Contact() { Name = "Inser Solutions Pvt. Ltd. ", Email = "saravanan.s@insersolutions.com", Url = "http://www.insersolutions.com/" }
                });

                // Swagger 2.+ support
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);
                c.DocumentFilter<AuthTokenOperation>();
                c.OperationFilter<AddAuthorizationHeaderParameterOperationFilter>();
            });

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromHours(1);
                //options.IdleTimeout = TimeSpan.FromSeconds(10);
                //options.Cookie.HttpOnly = true;
            });

            //JWT Configuration
            ConfigureJwtAuthService(services);
            // To avoid SSL certificate.
            services.AddHttpClient("HttpClientWithSSLUntrusted").ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ClientCertificateOptions = ClientCertificateOption.Manual,
                ServerCertificateCustomValidationCallback =
            (httpRequestMessage, cert, cetChain, policyErrors) =>
            {
                return true;
            }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");
            app.UseMiddleware<TokenProviderMiddleware>();
            app.UseMiddleware<RefreshTokenProviderMiddleware>();
            app.UseAuthentication();

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseCookiePolicy();

            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "VM API Documentation - V1");
                //c.DocumentTitle = "Title Documentation";
                //c.DocExpansion(DocExpansion.None);
                c.DefaultModelExpandDepth(2);
                c.DefaultModelsExpandDepth(-1);
                c.DisplayOperationId();
                c.DocExpansion(DocExpansion.List);
                c.SupportedSubmitMethods(SubmitMethod.Get, SubmitMethod.Post, SubmitMethod.Put, SubmitMethod.Delete);
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }

        public void ConfigureJwtAuthService(IServiceCollection services)
        {
            services.AddAuthentication(sharedOptions =>
            {
                sharedOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                sharedOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                //cfg.SaveToken = true; //TODO check

                cfg.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["JWTSettings:Issuer"],
                    ValidAudience = Configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["JWTSettings:SecretKey"]))
                };

                cfg.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        //Console.WriteLine("OnAuthenticationFailed: " +
                        //    context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        //Console.WriteLine("OnTokenValidated: " +
                        //    context.SecurityToken);
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
