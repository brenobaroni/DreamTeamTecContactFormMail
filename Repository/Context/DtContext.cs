using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Repository.Domain;
namespace Repository.Context
{
    public class DtContext : DbContext
    {
        IConfiguration _configuration;
        public DbSet<Leads> Leads { get; set; }
        public DtContext(DbContextOptions<DtContext> options) : base(options) { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var connectionString = _configuration.GetConnectionString("WebApiDatabase");
        //    optionsBuilder.UseMySQL(connectionString);
        //}
    }
}
