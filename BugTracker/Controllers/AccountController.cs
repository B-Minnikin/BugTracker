using BugTracker.Models.ProjectInvitation;
using BugTracker.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using BugTracker.Models;
using BugTracker.Models.Authorization;
using BugTracker.Models.Messaging;

namespace BugTracker.Controllers;

public class AccountController : Controller
{
	private readonly ILogger<AccountController> logger;
	private readonly ApplicationUserManager userManager;
	private readonly SignInManager<ApplicationUser> signInManager;
	private readonly IProjectInviter projectInvitation;
	private readonly IWebHostEnvironment webHostEnvironment;
	private readonly IEmailHelper emailHelper;

	public AccountController(ILogger<AccountController> logger,
							 ApplicationUserManager userManager,
							 SignInManager<ApplicationUser> signInManager,
							 IProjectInviter projectInvitation,
							 IWebHostEnvironment webHostEnvironment,
							 IEmailHelper emailHelper)
	{
		this.logger = logger;
		this.userManager = userManager;
		this.signInManager = signInManager;
		this.projectInvitation = projectInvitation;
		this.webHostEnvironment = webHostEnvironment;
		this.emailHelper = emailHelper;
	}

	[HttpGet]
	[AllowAnonymous]
	public ViewResult Login()
	{
		return View();
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> Login(LoginViewModel model)
	{
		if (ModelState.IsValid)
		{
			var user = await userManager.FindByEmailAsync(model.Email);
			if (user != null && !user.EmailConfirmed && (await userManager.CheckPasswordAsync(user, model.Password)))
			{
				ModelState.AddModelError(string.Empty, "Email not confirmed");
				return View(model);
			}

			var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: true, false);

			if (result.Succeeded)
			{
				return RedirectToAction("Index", "Home");
				//return RedirectToAction("Projects", "Projects");
			}

			ModelState.AddModelError(string.Empty, "Invalid login attempt");
		}

		return View(model);
	}

	[HttpGet]
	[AllowAnonymous]
	public ViewResult Register()
	{
		return View();
	}

	[HttpPost]
	[AllowAnonymous]
	public async Task<IActionResult> Register(RegisterViewModel model)
	{
		if (ModelState.IsValid)
		{
			var user = new ApplicationUser
			{
				UserName = model.Email,
				Email = model.Email,
				NormalizedEmail = model.Email.ToUpper()
			};
			var result = await userManager.CreateAsync(user, model.Password);

			if (result.Succeeded)
			{
				var createdUser = await userManager.FindByEmailAsync(user.Email);
				logger.LogInformation($"New user registered. ID: {createdUser.Id}, Name: {createdUser.UserName}");

				await GenerateConfirmationEmail(createdUser);

				// ---- fulfil stored project invitations if they exist
				await projectInvitation.AddUserToProjectMemberRoleForAllPendingInvitations(createdUser.Email);

				return View("RegistrationComplete");
			}

			AddErrorToModelState(result);
		}

		return View(model);
	}

	private async Task GenerateConfirmationEmail(ApplicationUser user)
	{
		var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
		var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token }, Request.Scheme);

		if (webHostEnvironment.IsDevelopment())
		{
			logger.LogInformation($"Email confirmation link >>{confirmationLink}");
		}

		const string subject = "Verify your email address";
		var messageBody = "To activate your account, please click on the following link:\n\n" + confirmationLink;
		emailHelper.Send(user.UserName, user.Email, subject, messageBody);
	}

	[HttpGet]
	[AllowAnonymous]
	public async Task<IActionResult> ConfirmEmail(string userId, string token)
	{
		if(userId == null || token == null)
		{
			logger.LogWarning($"Confirm email failed. Token or user ID is null");
			return RedirectToAction("Index", "Home");
		}

		var user = await userManager.FindByIdAsync(userId);
		if (user == null)
		{
			// generate error message
			logger.LogError($"Failed to find user for email confirmation: {userId}");
			return View("Error");
		}

		var result = await userManager.ConfirmEmailAsync(user, token);
		if (result.Succeeded)
		{
			logger.LogInformation($"User [{user.Id}, {user.UserName}] has confirmed their email successfully");
			return View("ConfirmEmail");
		}
		else
		{
			logger.LogError($"User [{user.Id}, {user.UserName}] failed to confirm email address");
			return View("Error");
		}
	}

	[HttpPost]
	public async Task<IActionResult> LogOut()
	{
		await signInManager.SignOutAsync();

		return RedirectToAction("Index", "Home");
	}

	[HttpGet]
	[Authorize]
	public ViewResult EditProfile()
	{
		return View();
	}

	[HttpPost]
	[Authorize]
	public ViewResult EditProfile(EditProfileViewModel profile)
	{
		return View();
	}

	[HttpGet]
	[AllowAnonymous]
	public IActionResult ForgotPassword()
	{
		return View();
	}

	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel viewModel)
	{
		if (ModelState.IsValid)
		{
			var user = await userManager.FindByEmailAsync(viewModel.Email);
			if (user == null)
				return RedirectToAction("ForgotPasswordConfirmation");

			await GenerateForgotPasswordEmail(user);

			return RedirectToAction("ForgotPasswordConfirmation");
		}

		return View(viewModel);
	}

	public IActionResult ForgotPasswordConfirmation()
	{
		return View();
	}

	[HttpGet]
	[AllowAnonymous]
	public IActionResult ResetPassword(string token, string email)
	{
		if(token == null || email == null)
		{
			ModelState.AddModelError("", "Invalid password reset token");
			return View();
		}
		
		var model = new ResetPasswordViewModel { Token = token, Email = email };
		return View(model);
	}

	[HttpPost]
	[AllowAnonymous]
	[ValidateAntiForgeryToken]
	public async Task<IActionResult> ResetPassword(ResetPasswordViewModel viewModel)
	{
		var user = await userManager.FindByEmailAsync(viewModel.Email);

		if(user != null)
		{
			var result = await userManager.ResetPasswordAsync(user, viewModel.Token, viewModel.Password);

			if (result.Succeeded)
			{
				return View("ResetPasswordConfirmation");
			}

			foreach(var error in result.Errors)
			{
				ModelState.AddModelError("", error.Description);
			}
			return View(viewModel);
		}

		return View("ResetPasswordConfirmation");
	}

	private async Task GenerateForgotPasswordEmail(ApplicationUser user)
	{
		var token = await userManager.GeneratePasswordResetTokenAsync(user);
		var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = user.Email, token }, Request.Scheme);

		const string subject = "Reset your password request";
		var messageBody = $"Hi {user.UserName}," +
			$"\n\nSomeone recently requested that the password for your BugTracker account be reset. " +
			$"If you didn't request this, then ignore this message. Please click the following link to reset your password:\n\n" + passwordResetLink;
		emailHelper.Send(user.UserName, user.Email, subject, messageBody);
	}

	private void AddErrorToModelState(IdentityResult result)
	{
		foreach(var error in result.Errors)
		{
			ModelState.AddModelError(string.Empty, error.Description);
		}
	}
}
