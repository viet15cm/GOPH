
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOPH.Entites
{
    public class Product
    {
        public Product() 
        {
            Id = Guid.NewGuid().ToString();
            DateCreate = DateTime.Now;
        }

        [Key]
        public string Id { get; set; }

        [Display(Name="Mã vạch")]
        public string Code { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        public string Name { get; set; }

        [Display(Name = "Tên hiển thị")]
        public string DisplayName { get; set; }

        [Display(Name = "Giá bán")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        public decimal Price { get; set; }

        [Range(0, 100, ErrorMessage = "Giảm giá khoảng từ 0% đến 100%")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        [Display(Name = "Giảm giá (%)")]
        public int Promotion { get; set; }

        [Display(Name="Mô tả")]
        [DataType(DataType.Text)]
        public string Description { set; get; }
       

        [Display(Name = "Giá vốn")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        public decimal CapitalPrice { get; set;}

        [Display(Name="Ảnh")]
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

        [Display(Name = "Hiện giá")]
        public bool IsPrice { get; set; }

        [Display(Name = "Sự kiện")]
        public bool IsEvent { get; set; }

        [DataType(DataType.Text)]
        [Display(Name="Nội dung")]
        public string Content { get; set; }

        [Display(Name="Ngày tạo")]
        public DateTime DateCreate { get; set; }

        [Display(Name="Ngày cập nhật")]
        public DateTime DateUpdate { get; set; }
      
        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        public virtual ICollection<Image> Images { get; set; }

        public virtual  Wholesale Wholesale { get; set; }

        public string EventId { get; set; }
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }
        public string HangHoa { get; set; }

        public string NhomHang { get; set; }


    }
}
