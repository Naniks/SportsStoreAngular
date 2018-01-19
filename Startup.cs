using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SpaServices.Webpack;
using SportsStore.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.Extensions.Logging;

namespace SportsStore {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        public void ConfigureServices(IServiceCollection services) {

            services.AddDbContext<IdentityDataContext>(options =>
                options.UseSqlServer(Configuration
                    ["Data:Identity:ConnectionString"]));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<IdentityDataContext>()
                .AddDefaultTokenProviders();

            services.AddDbContext<DataContext>(options =>
               options.UseSqlServer(Configuration
                  ["Data:Products:ConnectionString"]));

            services.AddMvc().AddJsonOptions(opts => {
                opts.SerializerSettings.ReferenceLoopHandling
                    = ReferenceLoopHandling.Serialize;
                opts.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.AddMemoryCache();
            services.AddSession();

            services.AddAntiforgery(options => {
                options.HeaderName = "X-XSRF-TOKEN";
            });

            //services.Configure<IdentityOptions>(config => {
            //    config.Cookies.ApplicationCookie.Events =
            //    new CookieAuthenticationEvents {
            //        OnRedirectToLogin = context => {
            //            if (context.Request.Path.StartsWithSegments("/api")
            //                    && context.Response.StatusCode == 200) {
            //                context.Response.StatusCode = 401;
            //            }
            //            else {
            //                context.Response.Redirect(context.RedirectUri);
            //            }
            //            return Task.FromResult<object>(null);
            //        }
            //    };
            //});
        }


        public void Configure(IApplicationBuilder app,
                 IHostingEnvironment env, ILoggerFactory loggerFactory,
                 IAntiforgery antiforgery) {
            app.UseDeveloperExceptionPage();
            app.UseWebpackDevMiddleware(new WebpackDevMiddlewareOptions {
                HotModuleReplacement = true
            });

            app.UseStaticFiles();
            app.UseSession();
            app.UseAuthentication();
            app.UseIdentity();

            app.Use(nextDelegate => context => {
                if (context.Request.Path.StartsWithSegments("/api")
                || context.Request.Path.StartsWithSegments("/")) {
                    context.Response.Cookies.Append("XSRF-TOKEN",
                    antiforgery.GetAndStoreTokens(context).RequestToken);
                }
                return nextDelegate(context);
            });

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapSpaFallbackRoute("angular-fallback",
                    new { controller = "Home", action = "Index" });
            });

            SeedData.SeedDatabase(app);
            IdentitySeedData.SeedDatabase(app);
        }
    }
}
