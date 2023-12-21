using GOPH.Entites;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GOPH.Dto
{
    public class WholesaleForCreateDto
    {
        

        [Display(Name = "Giá sỉ")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Giảm giá khoảng từ 0% đến 100%")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        [Display(Name = "Giảm giá (%)")]
        public int Promotion { get; set; }

        public string ProductId { get; set; }

      
    }
}
