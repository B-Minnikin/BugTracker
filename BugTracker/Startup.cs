using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Models.Database;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartBreadcrumbs.Extensions;

namespace BugTracker
{
	public class Startup
	{
		public IConfiguration Configuration { get; }
		public static string ConnectionString { get; private set; }

		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
			ConnectionString = this.Configuration.GetConnectionString("DBConnectionString");
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllersWithViews();
			services.AddScoped<IProjectRepository, DapperProjectRepository>();

			services.AddTransient<IUserStore<IdentityUser>, UserStore>();
			services.AddTransient<IRoleStore<IdentityRole>, RoleStore>();
			services.AddIdentity<IdentityUser, IdentityRole>()
				.AddUserManager<ApplicationUserManager>();

			services.AddDistributedMemoryCache();
			services.AddSession(options =>
			{
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});
			
			services.AddBreadcrumbs(GetType().Assembly);
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseAuthentication();

			app.UseRouting();

			app.UseAuthorization();
			app.UseSession();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
			    name: "default",
			    pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
