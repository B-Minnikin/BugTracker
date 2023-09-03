using System.ComponentModel.DataAnnotations.Schema;

namespace BugTracker.Models.ProjectInvitation;

public class PendingProjectInvitation
{
    [ForeignKey("Project")]
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public string EmailAddress { get; set; }
}
