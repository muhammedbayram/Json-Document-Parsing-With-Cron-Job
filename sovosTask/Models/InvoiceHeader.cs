using System.ComponentModel.DataAnnotations;

namespace sovosTask.Models
{
    public class InvoiceHeader
    {
        [Key]
        public string InvoiceId { get; set; }
        public string SenderTitle { get; set; }
        public string ReceiverTitle { get; set; }
        public DateTime Date { get; set; }

        public virtual List<InvoiceLine> InvoiceLines { get; set; }
    }
}
