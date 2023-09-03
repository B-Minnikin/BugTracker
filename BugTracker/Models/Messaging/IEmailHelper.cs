
namespace BugTracker.Models.Messaging;

public interface IEmailHelper
{
	void Send(string userName, string emailAddress, string subject, string messageBody);
}
