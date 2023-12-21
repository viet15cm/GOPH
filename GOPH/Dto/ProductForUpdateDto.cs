using GOPH.ModelValidation;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace GOPH.Dto
{
    public class ProductForUpdateDto
    {
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        [Display(Name = "Mã")]
        public string Id { get; set; }
      
        [Display(Name = "Mã Vạch")]
        [NumberCodeConfirmed]
        public string Code { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        public string Name { get; set; }

        [Display(Name = "Tên hiển thị")]
        public string DisplayName { get; set; }

        [Display(Name = "Giá vốn")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        public decimal CapitalPrice { get; set; }

        [Display(Name = "Giá bán")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        public decimal Price { get; set; }

        [Display(Name = "Ngày cập nhật")]
        public DateTime DateUpdate { get; set; }
        
        [Range(0, 100, ErrorMessage = "Giảm giá khoảng từ 0% đến 100%")]
        [Display(Name = "Giảm giá (%)")]
        [IntValidation]
        public int Promotion { get; set; }

        [Display(Name = "Hiện giá")]
        public string IsPrice { get; set; }

        public string Hot { get; set; }


        [Display(Name = "Sự kiện")]
        public string IsEvent { get; set; }

        [Display(Name = "Loại")]
        public string CommodidyId { get; set; }

        [Display(Name = "Nhóm")]
        public string CommodityGroupId { get; set; }
        public bool GetIsPice()
        {
            return bool.Parse(this.IsPrice);
        }

        public bool GetIsHot()
        {
            return bool.Parse(this.Hot);
        }

        public bool GetIsEvent()
        {
            return bool.Parse(this.IsEvent);
        }
    }
}
