using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
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

		public EmailHelper(IConfiguration configuration)
		{
			this.configuration = configuration;
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
			string username = "NebBugTracker";
			string pwd = configuration.GetSection("EmailSettings").GetSection("Password").Value;

			client.Connect(smtpAddress, port, MailKit.Security.SecureSocketOptions.StartTls);
			client.Authenticate(username, pwd);

			client.Send(message);
			client.Disconnect(true);
			client.Dispose();
		}
	}
}
