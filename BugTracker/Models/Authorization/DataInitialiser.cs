using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace BugTracker.Models.Authorization;
	
public enum Roles
{
	Superadministrator,
	Administrator,
	Member
}

public static class DataInitialiser
{
	public static async Task SeedRoles(IServiceProvider serviceProvider)
	{
		using var scope = serviceProvider.CreateScope();
		var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

		foreach (var role in Enum.GetNames(typeof(Roles)))
		{
			if(!await roleManager.RoleExistsAsync(role))
			{
				await roleManager.CreateAsync(new IdentityRole(role));
			}
		}
	}
}
