using Domain.Entities;
using Domain.Models;
using Microsoft.Extensions.Options;
using Repository.Domain;
using Repository.Repository.Contracts;
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
        private readonly ILeadsRepository _leadsRepository;

        public EmailService(IOptions<EmailSettings> emailSettings, ILeadsRepository leadsRepository)
        {
            _emailSettings = emailSettings.Value;
            _leadsRepository = leadsRepository;

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
                        mailMessage.To.Add(new MailAddress(_emailSettings.To));
                        mailMessage.Subject = "Contato Recebido";
                        mailMessage.IsBodyHtml = true;
                        string msgBody =
                            $"<h1>Contato Recebido</h1>" +
                            $"<p><b>Nome: </b> {model.Nome} </p>" +
                            $"<p><b>E-mail: </b> {model.Email}</p>" +
                            $"<p><b>Telefone: </b> {model.Telefone}</p>" +
                            $"<p><b>Mensagem: </b> {model.Mensagem}</p>" +
                            $"<p><b>Horário: </b> {DateTime.Now.ToString("dd/MM/yyy HH:mm:ss")}</p>";
                        mailMessage.Body = msgBody;
                        mailMessage.IsBodyHtml = true;
                        smtp.Port = _emailSettings.Port;
                        smtp.Host = _emailSettings.Host;
                        smtp.UseDefaultCredentials = false;
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential(_emailSettings.Mail, _emailSettings.Password, _emailSettings.Host);
                        smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                        
                        await smtp.SendMailAsync(mailMessage);

                        await SendCompletedCallback(model);

                        return true;

                    }catch(Exception ex)
                    {
                        return false;
                    }

                }
            }

        }

        private async Task SendCompletedCallback(ContactFormModel model)
        {
            var lead = new Leads()
            {
                Nome = model.Nome,
                Email = model.Email,
                Mensagem = model.Mensagem,
                Telefone = model.Telefone,
            };

            await _leadsRepository.SaveAsync(lead);
        }
    }
}
