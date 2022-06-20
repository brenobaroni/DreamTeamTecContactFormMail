using Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DreamTeamTecContactFormMail.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactFormEmailController : ControllerBase
    {
        public readonly IEmailService _emailService;

        public ContactFormEmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost]
        [Route("EnviarForm")]
        public async Task<IActionResult> EnviarContactForm([FromBody] ContactFormModel contactFormModel)
        {
            try
            {
                var properties = contactFormModel.GetType().GetProperties();

                foreach (var prop in properties)
                {
                    if(prop.GetValue(contactFormModel) is null or "")
                    {
                        return BadRequest($"É obrigatório informar {prop.Name}");
                    }
                }

                return Ok(await _emailService.EnviarFormEmailAsync(contactFormModel));
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
