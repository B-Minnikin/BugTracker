using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace BugTracker.Models.Messaging;

public class EmailHelper : IEmailHelper
{
	private readonly IConfiguration configuration;
	private readonly ILogger<EmailHelper> logger;

	public EmailHelper(IConfiguration configuration,
							ILogger<EmailHelper> logger)
	{
		this.configuration = configuration;
		this.logger = logger;
	}

	public void Send(string userName, string emailAddress, string subject, string messageBody)
	{
		var message = new MimeMessage();
		var from = new MailboxAddress("BugTracker", "NebBugTracker@gmail.com");
		message.From.Add(from);

		var to = new MailboxAddress(userName, emailAddress);
		message.To.Add(to);

		message.Subject = subject;

		message.Body = GenerateMessageBody(messageBody);
		SendMessage(message);
	}

	private static MimeEntity GenerateMessageBody(string messageBody)
	{
		var bodyBuilder = new BodyBuilder
		{
			TextBody = messageBody
		};
		return bodyBuilder.ToMessageBody();
	}

	private void SendMessage(MimeMessage message)
	{
		var client = new SmtpClient();
		const string smtpAddress = "smtp.gmail.com";
		const int port = 587;

		try
		{
			var username = configuration.GetSection("EmailSettings").GetSection("SenderName").Value;
			var pwd = configuration.GetSection("EmailSettings").GetSection("Password").Value;

			client.Connect(smtpAddress, port, MailKit.Security.SecureSocketOptions.StartTls);
			client.Authenticate(username, pwd);

			client.Send(message);
		}
		catch(MailKit.Security.AuthenticationException e)
		{
			logger.LogError(e.Message);
		}
		finally
		{
			client.Disconnect(true);
			client.Dispose();
		}
	}
}
