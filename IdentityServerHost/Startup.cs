using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public static HttpMessageHandler Handler { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(Config.IdentityResources)
                .AddInMemoryClients(Config.Clients)
                .AddInMemoryApiResources(Config.Apis)
                .AddTestUsers(TestUsers.Users);

            services.AddAuthentication()
                .AddJwtBearer(jwt =>
                {
                    // defaults as they were
                    jwt.Authority = "http://localhost:5000/";
                    jwt.RequireHttpsMetadata = false;
                    jwt.Audience = "api1";
                    // if static handler is null don't change anything, otherwise assume integration test.
                    if(Handler == null)
                    {
                        jwt.BackchannelHttpHandler = Handler;
                        jwt.Authority = "http://localhost/";
                    }
                });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseIdentityServer();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
