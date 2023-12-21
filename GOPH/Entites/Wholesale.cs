using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOPH.Entites
{
    public class Wholesale
    {
        public Wholesale() 
        {
            Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }

        [Display(Name ="Giá sỉ")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Giảm giá khoảng từ 0% đến 100%")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        [Display(Name = "Giảm giá (%)")]
        public int Promotion { get; set; }

        public string ProductId { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

    }
}
