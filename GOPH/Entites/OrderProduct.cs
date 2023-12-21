using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOPH.Entites
{
    public class OrderProduct
    {
        public string OderId { get; set; }

        [ForeignKey("OderId")]
        public virtual Order Order { get; set; }

        public string ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [Display(Name = "Số lượng")]
        [Range(1, int.MaxValue, ErrorMessage ="{0} phải lớn hơn {1}")]
        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public int Promotion { get; set; }

        public bool IsWholesale { get; set; }

    }
}
