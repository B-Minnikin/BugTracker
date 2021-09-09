using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Models.Database;
using BugTracker.Models.ProjectInvitation;
using BugTracker.Repository;
using BugTracker.Repository.DapperRepositories;
using BugTracker.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
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
			services.AddSingleton<IConfiguration>(Configuration);
			services.AddTransient<IEmailHelper, EmailHelper>();

			ConfigureRepositories(services, Configuration);

			services.AddScoped<ISubscriptions, Subscriptions>();
			services.AddScoped<IProjectInviter, ProjectInviter>();

			services.AddTransient<IUserStore<IdentityUser>, UserStore>();
			services.AddTransient<IRoleStore<IdentityRole>, RoleStore>();
			services.AddIdentity<IdentityUser, IdentityRole>(options =>
				options.SignIn.RequireConfirmedEmail = true
			)
				.AddDefaultTokenProviders()
				.AddUserManager<ApplicationUserManager>();

			services.AddAuthorization(options =>
			{
				options.AddPolicy("CanAccessProjectPolicy", policy =>
					policy.Requirements.Add(new	ProjectAccessRequirement()));
				options.AddPolicy("ProjectAdministratorPolicy", policy =>
					policy.Requirements.Add(new	ProjectAdministratorRequirement()));
				options.AddPolicy("CanModifyReportPolicy", policy =>
					policy.Requirements.Add(new ModifyReportRequirement()));
				options.AddPolicy("CanModifyCommentPolicy", policy =>
					policy.Requirements.Add(new ModifyCommentRequirement()));
				options.AddPolicy("CanModifyProfilePolicy", policy =>
					policy.Requirements.Add(new ModifyProfileRequirement()));
			});

			services.AddSingleton<IAuthorizationHandler, ProjectAccessAuthorizationHandler>();
			services.AddSingleton<IAuthorizationHandler, ProjectAdministratorAuthorizationHandler>();
			services.AddSingleton<IAuthorizationHandler, ModifyReportAuthorizationHandler>();
			services.AddSingleton<IAuthorizationHandler, ModifyCommentAuthorizationHandler>();
			services.AddSingleton<IAuthorizationHandler, ModifyProfileAuthorizationHandler>();

			services.AddDistributedMemoryCache();
			services.AddSession(options =>
			{
				options.Cookie.HttpOnly = true;
				options.Cookie.IsEssential = true;
			});
			
			services.AddBreadcrumbs(GetType().Assembly);
		}

		private void ConfigureRepositories(IServiceCollection services, IConfiguration configuration)
		{
			string connectionString = configuration.GetConnectionString("DBConnectionString");

			services.AddTransient<IProjectRepository, DapperProjectRepository>(s => new DapperProjectRepository(connectionString));
			services.AddTransient<IMilestoneRepository, DapperMilestoneRepository>(s => new DapperMilestoneRepository(connectionString));
			services.AddTransient<IBugReportRepository, DapperBugReportRepository>(s => new DapperBugReportRepository(connectionString));
			services.AddTransient<IBugReportStatesRepository, DapperBugReportStatesRepository>(s => new DapperBugReportStatesRepository(connectionString));
			services.AddTransient<IUserSubscriptionsRepository, DapperUserSubscriptionsRepository>(s => new DapperUserSubscriptionsRepository(connectionString));
			services.AddTransient<IActivityRepository, DapperActivityRepository>(s => new DapperActivityRepository(connectionString));
			services.AddTransient<ICommentRepository, DapperCommentRepository>(s => new DapperCommentRepository(connectionString));
			services.AddTransient<IProjectInvitationsRepository, DapperProjectInvitationsRepository>(s => new DapperProjectInvitationsRepository(connectionString));
			services.AddTransient<ISearchRepository, DapperSearchRepository>(s => new DapperSearchRepository(connectionString));
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
				app.UseStatusCodePagesWithReExecute("/Error/{0}");
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}
			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthentication();
			app.UseAuthorization();
			app.UseSession();

			DataInitialiser.SeedRoles(app.ApplicationServices).Wait();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
			    name: "default",
			    pattern: "{controller=Home}/{action=Index}/{id?}");
			});
		}
	}
}
