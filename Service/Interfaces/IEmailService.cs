using Domain.Models;
using System.Threading.Tasks;

namespace Service.Interfaces
{
    public interface IEmailService
    {
        Task<bool> EnviarFormEmailAsync(ContactFormModel model);
    }
}
