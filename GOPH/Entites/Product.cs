using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOPH.Entites
{
    public class Product
    {
        public Product() 
        {
            Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }
        public string Code { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        public string Name { get; set; }

        [Display(Name = "Giá bán")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Giảm giá khoảng từ 0% đến 100%")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        [Display(Name = "Giảm giá (%)")]
        public int Promotion { get; set; }

        [Display(Name="Mô tả")]
     
        [DataType(DataType.Text)]
        [StringLength(1000, ErrorMessage = "{0} tối đa 1000 ký tự ")]
        public string Description { set; get; }
       

        [Display(Name = "Giá vốn")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        public decimal CapitalPrice { get; set;}
        public string UrlImage { get; set; }

        public bool Hot { get; set; }

        [Display(Name="Loại")]
        public string CommodidyId { get; set; }

        [ForeignKey("CommodidyId")]
        public virtual Commodity Commodity { get; set; }

        [Display(Name = "Nhóm")]
        public string CommodityGroupId { get; set; }

        [ForeignKey("CommodityGroupId")]
        public virtual CommodityGroup CommodityGroup { get; set; }

        [Display(Name ="Hiện giá")]
        public bool IsPrice { get; set; }

        [DataType(DataType.Text)]
        [Display(Name="Nội dung")]
        public string Content { get; set; }

        public string HangHoa { get; set; }

        public string NhomHang { get; set; }

        public int Rank { get; set; }

    }
}
