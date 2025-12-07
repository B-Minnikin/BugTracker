using BugTracker.Repository.Interfaces;
using BugTracker.Services;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Models.Messaging;

namespace BugTracker.Models.Subscription;

public class Subscriptions : ISubscriptions
{
    private readonly IProjectRepository projectRepository;
    private readonly IBugReportRepository bugReportRepository;
    private readonly IUserSubscriptionsRepository userSubscriptionsRepository;
    private readonly IEmailHelper emailHelper;
    private readonly UserManager<IdentityUser> userManager;

    public Subscriptions(IProjectRepository projectRepository,
        IBugReportRepository bugReportRepository,
        IUserSubscriptionsRepository userSubscriptionsRepository,
        IEmailHelper emailHelper,
        UserManager<IdentityUser> userManager)
    {
        this.projectRepository = projectRepository;
        this.bugReportRepository = bugReportRepository;
        this.userSubscriptionsRepository = userSubscriptionsRepository;
        this.emailHelper = emailHelper;
        this.userManager = userManager;
    }

    public async Task<bool> IsSubscribed(int userId, int bugReportId)
    {
        return await userSubscriptionsRepository.IsSubscribed(userId, bugReportId);
    }

    public async Task CreateSubscriptionIfNotSubscribed(int userId, int bugReportId)
    {
        if (!await IsSubscribed(userId, bugReportId))
        {
            await userSubscriptionsRepository.AddSubscription(userId, bugReportId);
        }
    }

    public async Task DeleteSubscription(int userId, int bugReportId)
    {
        if (await IsSubscribed(userId, bugReportId))
        {
            await userSubscriptionsRepository.DeleteSubscription(userId, bugReportId);
        }
    }

    public async Task NotifyBugReportStateChanged(BugState bugState, ApplicationLinkGenerator linkGenerator,
        int bugReportId)
    {
        var bugReportUrl =
            linkGenerator.GetPathByAction("ReportOverview", "BugReport", new { bugReportId });

        var subscribedUserIds = await userSubscriptionsRepository.GetAllSubscribedUserIds(bugState.BugReportId);

        var emailSubject = await ComposeBugStateEmailSubject(bugState);
        var emailMessage = await ComposeBugStateEmailMessage(bugState, bugReportUrl);

        foreach (var userId in subscribedUserIds)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (bugState.Author != user.UserName)
            {
                emailHelper.Send(user.UserName, user.Email, emailSubject, emailMessage);
            }
        }
    }

    public async Task NotifyBugReportNewComment(Comment comment, string bugReportUrl)
    {
        var subscribedUserIds = await userSubscriptionsRepository.GetAllSubscribedUserIds(comment.BugReportId);

        var bugReport = await bugReportRepository.GetById(comment.BugReportId);
        var project = await projectRepository.GetById(bugReport.ProjectId);
        var projectName = project.Name;
        var emailSubject = $"Bug report updated: {bugReport.Title}";
        var emailMessage =
            $"Project: {projectName}\nNew comment posted in bug report {bugReport.Title} by {comment.AuthorId}.\n" +
            $"Please <a href=\"{bugReportUrl}\">click here</a> to review new content.";

        foreach (var userId in subscribedUserIds)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());

            if (comment.AuthorId != int.Parse(user.Id))
            {
                emailHelper.Send(user.UserName, user.Email, emailSubject, emailMessage);
            }
        }
    }

    private async Task<string> ComposeBugStateEmailSubject(BugState bugState)
    {
        var bugReport = await bugReportRepository.GetById(bugState.BugReportId);
        var message = $"Bug report updated: {bugReport.Title}";

        return message;
    }

    private async Task<string> ComposeBugStateEmailMessage(BugState bugState, string bugReportUrl)
    {
        var bugReport = await bugReportRepository.GetById(bugState.BugReportId);
        var project = await projectRepository.GetById(bugReport.ProjectId);
        var projectName = project.Name;
        var stateName = bugState.StateType.ToString().First().ToString().ToUpper() +
                           bugState.StateType.ToString().Substring(1);

        var message = $"Project: {projectName}\n{bugReport.Title} state updated to {stateName}\n" +
                         $"Please <a href=\"{bugReportUrl}\">click here</a> to review new content.";

        return message;
    }
}
