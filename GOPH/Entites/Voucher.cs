using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOPH.Entites
{
    public class Voucher
    {
        public Voucher()
        {
            Code = Guid.NewGuid().ToString().Substring(24);
        }

        [Key]
        public string Code { get; set; }

        [Range(1000 , int.MaxValue, ErrorMessage ="{0} ít nhất là {1} vnd")]
        public int Price { get; set; }

        public string CustomerId { get; set; }

        [ForeignKey("CustomerId")]
        public AppUser User { get; set; }
        
        public string OrderId { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
       
    }
}
