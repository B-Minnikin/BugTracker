using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Models
{
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
			MimeMessage message = new MimeMessage();
			MailboxAddress from = new MailboxAddress("BugTracker", "NebBugTracker@gmail.com");
			message.From.Add(from);

			MailboxAddress to = new MailboxAddress(userName, emailAddress);
			message.To.Add(to);

			message.Subject = subject;

			message.Body = GenerateMessageBody(messageBody);
			SendMessage(message);
		}

		private MimeEntity GenerateMessageBody(string messageBody)
		{
			BodyBuilder bodyBuilder = new BodyBuilder();
			bodyBuilder.TextBody = messageBody;
			return bodyBuilder.ToMessageBody();
		}

		private void SendMessage(MimeMessage message)
		{
			SmtpClient client = new SmtpClient();
			string smtpAddress = "smtp.gmail.com";
			int port = 587;

			try
			{
				string username = configuration.GetSection("EmailSettings").GetSection("SenderName").Value;
				string pwd = configuration.GetSection("EmailSettings").GetSection("Password").Value;

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
}
