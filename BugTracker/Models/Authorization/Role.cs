using System.Collections.Generic;

namespace BugTracker.Models.Authorization;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string NormalisedName { get; set; }
    public string IdentityRoleId { get; set; }
    
    public ICollection<UserRole> UserRoles { get; set; }
}
