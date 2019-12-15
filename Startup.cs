using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using BeltExam.Models;
// using Microsoft.AspNetCore.Identity;

namespace BeltExam
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      // services.Configure<CookiePolicyOptions>(options =>
      // {
      //   // This lambda determines whether user consent for non-essential cookies is needed for a given request.
      //   options.CheckConsentNeeded = context => true;
      //   options.MinimumSameSitePolicy = SameSiteMode.None;
      // });

      // services.Configure<IdentityOptions>(options =>
      // {
      //   // Default Password settings.
      //   options.Password.RequireDigit = true;
      //   options.Password.RequireLowercase = true;
      //   options.Password.RequireNonAlphanumeric = true;
      //   options.Password.RequireUppercase = true;
      //   options.Password.RequiredLength = 8;
      //   options.Password.RequiredUniqueChars = 1;
      //   // Default User settings.
      //   options.User.AllowedUserNameCharacters =
      //     "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
      //   options.User.RequireUniqueEmail = true;
      // });

      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
      services.AddSession();
      string mySqlConnection = Configuration["DBInfo:ConnectionString"];
      services.AddDbContext<BeltExamContext>(options => options.UseMySql(mySqlConnection));
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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

      app.UseStaticFiles();
      // app.UseCookiePolicy();
      app.UseSession();
      // app.UseIdentity();
      app.UseMvc(routes =>
      {
        routes.MapRoute(
          name: "default",
          template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
