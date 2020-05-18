using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CVweb.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using ReflectionIT.Mvc.Paging;

namespace CVweb
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public Startup(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContextPool<DBapp>(op =>
            op.UseSqlite(Configuration.GetConnectionString("con")));
            services.AddIdentity<IdentityUser, IdentityRole>(op =>
            {
                op.Password.RequireNonAlphanumeric = false;
                op.Password.RequireUppercase = false;
            }
            
            )
                .AddEntityFrameworkStores<DBapp>()
                .AddDefaultTokenProviders();

            services.AddSingleton<IFileProvider>(
            new PhysicalFileProvider(
                Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

            services.AddMvc();
            services.AddPaging();
            services.AddAuthentication().AddGoogle(op =>
            {
                op.ClientId = "1023992901916-snn3k2mvd21odetlcne7iu648ksts0mo.apps.googleusercontent.com";
                op.ClientSecret = "19j_GEPadG5JYq8W1JQD9Me-";
            });
            services.AddScoped<CVaction, CVaction>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }           
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute(); 

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync(Configuration.GetConnectionString("con"));
            });
        }
    }
}
