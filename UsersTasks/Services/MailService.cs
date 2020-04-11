using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using UsersTasks.Models;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace UsersTasks.Services
{
    public class MailService
    {
        private readonly MyConfiguration configuration;

        public MailService(MyConfiguration configuration)
        {
            this.configuration = configuration;
        }

        //
        // Summary:
        //     Builds the message's body.
        //
        // Parameters:
        //   tasks:
        //     A List of tasks to build a message from.
        //
        //   messagePrefix:
        //     A prefix for the message's body.
        //
        //   messageSufix:
        //     A sufix for the message's body.
        //
        // Returns:
        //     A string containing a body of Email message
        //
        public string BuildMessageBodyFromTasks(List<Models.Task> tasks, string messagePrefix, string messageSufix)
        {
            try
            {
                string messageBody = messagePrefix;

                foreach (Models.Task task in tasks)
                {
                    messageBody += task.TaskName + "\n======\n";
                    messageBody += "ID: " + task.TaskId + "\n";
                    messageBody += "Status: " + (task.TaskStatus == 1 ? "Done" : "To be done") + "\n";
                    messageBody += "Is Active: " + (task.TaskStatus == 1 ? "Active" : "Not Active") + "\n";
                    messageBody += "Priority: " + task.Priority + "th" + "\n";
                    messageBody += "Target finish date: " + task.TargetFinishDate + "\n";
                    messageBody += "\n\n";
                }

                messageBody += messageSufix;

                return messageBody;
            }
            catch (Exception err)
            {
                throw err;
            }
        }

        //
        // Summary:
        //     Gets all the required data and sending an Email message using a SmtpClient.
        //
        // Parameters:
        //   toName:
        //     The name of the addressed person.
        //
        //   toAddress:
        //     The mail address of the addressed person.
        //
        //   messageSubject:
        //     The subject of the Email message.
        //
        //   messageBody:
        //     The body of the Email message.
        //
        // Returns:
        //     True if the message was sent successfully
        //
        public bool SendEmail(string toName, string toAddress, string messageSubject, string messageBody)
        {
            try
            {
                var message = new MimeMessage();

                message.From.Add(new MailboxAddress(configuration.email.fromName, configuration.email.fromAddress));

                message.To.Add(new MailboxAddress(toName, toAddress));

                message.Subject = messageSubject;

                message.Body = new TextPart("Plain")
                {
                    Text = messageBody
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(configuration.smtp.host, configuration.smtp.port, configuration.smtp.enableSsl);

                    client.Authenticate(configuration.smtp.username, configuration.smtp.password);

                    client.Send(message);

                    client.Disconnect(true);
                }

                return true;
            }
            catch (Exception err)
            {
                throw err;
            }
        }
    }
}
