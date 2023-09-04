using BugTracker.Database.Context;
using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Models.Messaging;
using BugTracker.Models.ProjectInvitation;
using BugTracker.Models.Subscription;
using BugTracker.Repository;
using BugTracker.Repository.EFCoreRepositories;
using BugTracker.Repository.Interfaces;
using BugTracker.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SmartBreadcrumbs.Extensions;

namespace BugTracker;

public class Startup
{
	private IConfiguration Configuration { get; }

	public Startup(IConfiguration configuration)
	{
		Configuration = configuration;
	}

	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddControllersWithViews();
		services.AddTransient<IEmailHelper, EmailHelper>();

		ConfigureRepositories(services);

		services.AddDbContext<ApplicationContext>(options =>
			options.UseSqlServer(Configuration.GetConnectionString("DBConnectionString")));

		services.AddTransient<ILinkGenerator, ApplicationLinkGenerator>();

		services.AddScoped<ISubscriptions, Subscriptions>();
		services.AddScoped<IProjectInviter, ProjectInviter>();

		services.AddTransient<IUserStore<ApplicationUser>, UserStore>();
		services.AddTransient<IRoleStore<IdentityRole>, RoleStore>();
		services.AddIdentity<ApplicationUser, IdentityRole>(options =>
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

		ConfigureAuthorizationHandlers(services);

		services.AddDistributedMemoryCache();
		services.AddSession(options =>
		{
			options.Cookie.HttpOnly = true;
			options.Cookie.IsEssential = true;
		});
		
		services.AddBreadcrumbs(GetType().Assembly);
	}

	private static void ConfigureRepositories(IServiceCollection services)
	{
		services.AddScoped<UserManager<ApplicationUser>, ApplicationUserManager>(serviceProvider => 
			ActivatorUtilities.CreateInstance<ApplicationUserManager>(serviceProvider, 
				serviceProvider.GetRequiredService<ApplicationContext>()));

		services.AddScoped<IMilestoneRepository, EfMilestoneRepository>();
		services.AddScoped<IBugReportRepository, EfBugReportRepository>();
		services.AddScoped<IBugReportStatesRepository, EfBugReportStatesRepository>();
		services.AddScoped<IUserSubscriptionsRepository, EfUserSubscriptionsRepository>();
		services.AddScoped<IActivityRepository, EfActivityRepository>();
		services.AddScoped<ICommentRepository, EfCommentRepository>();
		services.AddScoped<IProjectInvitationsRepository, EfProjectInvitationsRepository>();
		services.AddScoped<ISearchRepository, EfSearchRepository>();
	}

	private static void ConfigureAuthorizationHandlers(IServiceCollection services)
	{
		services.AddScoped<IAuthorizationHandler, ProjectAccessAuthorizationHandler>();
		services.AddScoped<IAuthorizationHandler, ProjectAdministratorAuthorizationHandler>();
		services.AddScoped<IAuthorizationHandler, ModifyReportAuthorizationHandler>();
		services.AddScoped<IAuthorizationHandler, ModifyCommentAuthorizationHandler>();
		services.AddScoped<IAuthorizationHandler, ModifyProfileAuthorizationHandler>();
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
