using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sovosTask.Models
{
    public class InvoiceLine
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]  //kayıt yaparken hata verdi identy özelliğini kapattım
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public string UnitCode { get; set; }
        public decimal UnitPrice { get; set; }

        public string InvoiceHeaderId { get; set; }
        public virtual InvoiceHeader InvoiceHeader { get; set; }
    }
}
