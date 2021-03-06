using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MQTTnet;
using MQTTnet.AspNetCore;
using MQTTnet.AspNetCore.Extensions;

// ????????? ??? ?????????????? Identity
using Plan_io_T.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

// ????????? ??? ??????????? ????????? ArduinoData
using Plan_io_T.Data;

namespace Plan_io_T {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {

            services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<User, IdentityRole>(opts => {
                opts.Password.RequireNonAlphanumeric = false;   // ????????? ?? ?? ?????????-???????? ???????
                opts.Password.RequireLowercase = true; // ????????? ?? ??????? ? ?????? ????????
                opts.Password.RequireUppercase = false; // ????????? ?? ??????? ? ??????? ????????
                opts.Password.RequireDigit = true; // ????????? ?? ?????
            })
        .AddEntityFrameworkStores<ApplicationContext>();

            services.AddControllersWithViews();

            services.AddDbContext<MvcDataContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MvcDataContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
