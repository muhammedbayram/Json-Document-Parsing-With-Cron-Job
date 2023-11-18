using Microsoft.EntityFrameworkCore;

namespace sovosTask.Models
{
    public class InvoiceDbContext : DbContext
    {

        public InvoiceDbContext(DbContextOptions<InvoiceDbContext> options) : base(options)
        {


        }

        public DbSet<InvoiceHeader> InvoiceHeaders { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }


    }
}
