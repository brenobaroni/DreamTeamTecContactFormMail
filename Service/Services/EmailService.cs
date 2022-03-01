using Domain.Entities;
using Domain.Models;
using Microsoft.Extensions.Options;
using Service.Interfaces;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Threading.Tasks;

namespace Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        static bool mailSent = false;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }
        public async Task<bool> EnviarFormEmailAsync(ContactFormModel model)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                using (SmtpClient smtp = new SmtpClient())
                {
                    try
                    {
                        mailMessage.From = new MailAddress(_emailSettings.Mail, _emailSettings.DisplayName);
                        mailMessage.To.Add(new MailAddress(_emailSettings.Mail));
                        mailMessage.Subject = "Contato Recebido";
                        mailMessage.IsBodyHtml = true;
                        string msgBody =
                            $"<h1>Contato Recebido</h1>" +
                            $"<p><b>Nome: </b> {model.Nome} </p>" +
                            $"<p><b>E-mail: </b> {model.Email}</p>" +
                            $"<p><b>Telefone: </b> {model.Telefone}</p>" +
                            $"<p><b>Mensagem: </b> {model.Mensagem}</p>";
                        mailMessage.Body = msgBody;
                        smtp.Port = _emailSettings.Port;
                        smtp.Host = _emailSettings.Host;
                        smtp.UseDefaultCredentials = false;
                        smtp.Credentials = new NetworkCredential(_emailSettings.Mail, _emailSettings.Password);
                        smtp.EnableSsl = true;
                        //smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                        await smtp.SendMailAsync(mailMessage);

                        smtp.SendCompleted += new
                                SendCompletedEventHandler(SendCompletedCallback);

                        return true;

                    }catch(Exception ex)
                    {
                        return false;
                    }

                }
            }

        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            mailSent = true;
        }
    }
}
