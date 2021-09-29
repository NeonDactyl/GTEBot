using BlazorGuessTheElo.Data;
using BlazorGuessTheElo.DataContext;
using BlazorGuessTheElo.Providers;
using BlazorGuessTheElo.Repositories;
using BlazorGuessTheElo.Repositories.Interfaces;
using BlazorGuessTheElo.Services;
using BlazorGuessTheElo.Services.Interfaces;
using BlazorGuessTheEloDiscord.OAuth2;
using Discord.Commands;
using Discord.Rest;
using Discord.WebSocket;
using MatBlazor;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorGuessTheElo
{
    public class Startup
    {
        private readonly string connString;
        public Startup(IConfiguration configuration)
        {
            //Configuration = configuration;

            //var host = Configuration["DBHOST"] ?? "192.168.1.171";
            //var port = Configuration["DBPORT"] ?? "3306";
            //var password = Configuration["MYSQL_PASSWORD"] ?? "RkR6Bq^rmu6f!%9JZdvN";
            //var userid = Configuration["MYSQL_USER"] ?? "eloBotUser";
            //var usersDataBase = Configuration["MYSQL_DATABASE"] ?? "elo";

            //connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
            Configuration = configuration;

            var host = Configuration["DBHOST"] ?? "localhost";
            var port = Configuration["DBPORT"] ?? "3306";
            var password = Configuration["MYSQL_PASSWORD"] ?? Configuration.GetConnectionString("MYSQL_PASSWORD");
            var userid = Configuration["MYSQL_USER"] ?? Configuration.GetConnectionString("MYSQL_USER");
            var usersDataBase = Configuration["MYSQL_DATABASE"] ?? Configuration.GetConnectionString("MYSQL_DATABASE");

            connString = $"server={host}; userid={userid};pwd={password};port={port};database={usersDataBase}";
            Console.WriteLine(connString);
            Console.WriteLine($"Host:{host}");
            Console.WriteLine($"Pport:{port}");
            Console.WriteLine($"Pass:{password}");
            Console.WriteLine($"User:{userid}");
            Console.WriteLine($"DB:{usersDataBase}");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Dashboard");
            });
            services.Configure<RouteOptions>(routeOptions =>
            {
            });
            services.AddServerSideBlazor();
            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = DiscordDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddDiscord(options =>
                {
                    options.AppId = Configuration.GetValue<string>("Discord:ClientId");
                    options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.AppSecret = Configuration.GetValue<string>("Discord:ClientSecret");
                    options.SaveTokens = true;
                    options.CorrelationCookie = new Microsoft.AspNetCore.Http.CookieBuilder
                    {
                        HttpOnly = false,
                        SameSite = Microsoft.AspNetCore.Http.SameSiteMode.None,
                        SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always,
                        Expiration = TimeSpan.FromMinutes(10)
                    };
                });
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders =
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            MySqlServerVersion version = new MySqlServerVersion(new Version(8, 0, 19));
            services.AddDbContext<EloSubmissionDatabaseContext>(options =>
            {
                options.UseMySql(connString, version).EnableDetailedErrors().EnableSensitiveDataLogging();
            }, ServiceLifetime.Singleton);
            services.AddHttpContextAccessor();
            services.AddHttpClient();

            //Scoped
            services.AddScoped<IDiscordManagementService, DiscordManagementService>();
            services.AddScoped<DiscordRestClient>();
            services.AddScoped<IEloSubmissionRepository, EloSubmissionRepository>();
            services.AddScoped<IAllowedChannelsRepository, AllowedChannelsRepository>();
            services.AddScoped<TokenProvider>();

            //Singleton
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<CommandService>();
            services.AddSingleton<ICommandHandlingService, CommandHandlingService>();
            services.AddSingleton<IEloSubmissionService, EloSubmissionService>();
            services.AddSingleton<IDatabaseChangesService, DatabaseChangesService>();
            services.AddSingleton<IMessageDeletionService, MessageDeletionService>();

            //HostedService
            services.AddMatBlazor();
            services.AddHostedService<EloBotService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // required in order to get https for OpenIdConnect
            // must come before app.UseAuthentication();
            var forwardedHeaderOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };

            forwardedHeaderOptions.KnownNetworks.Clear();
            forwardedHeaderOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(forwardedHeaderOptions);

            app.UseCookiePolicy(new CookiePolicyOptions()
            {
                MinimumSameSitePolicy = SameSiteMode.None
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
