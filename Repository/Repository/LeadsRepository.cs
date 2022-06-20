using Microsoft.EntityFrameworkCore;
using Repository.Context;
using Repository.Domain;
using Repository.Repository.Contracts;

namespace Repository.Repository
{
    public class LeadsRepository : ILeadsRepository
    {
        private DtContext _dtContext;
        public LeadsRepository(DtContext dtContext)
        {
            _dtContext = dtContext;
        }

        public async Task<int> SaveAsync(Leads lead)
        {
            try
            {

                var existsLead = _dtContext.Leads.FirstOrDefault(w => w.Email.Equals(lead.Email));

                if (existsLead == null)
                {
                    await _dtContext.Database.BeginTransactionAsync();
                    lead.RowDate = DateTime.Now;
                    await this._dtContext.Leads.AddAsync(lead);
                    await _dtContext.Database.CommitTransactionAsync();
                }
                else
                {
                    existsLead.Nome = lead.Nome;
                    existsLead.Telefone = lead.Telefone;
                    existsLead.Mensagem = lead.Mensagem;
                    lead.RowDate = DateTime.Now;
                }
                
                return await _dtContext.SaveChangesAsync();

            }
            catch (Exception)
            {
                _dtContext.Database.RollbackTransaction();
                return 0;
            }

        }
    }
}