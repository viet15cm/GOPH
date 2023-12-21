using System.ComponentModel.DataAnnotations;

namespace GOPH.Entites
{
  
    public class Event
    {
        [Key]
        public string Id { get; set; }

        [Display(Name="Tiêu đề")]
        [Required(ErrorMessage ="{0} không được bỏ trống")]
        public string Name { get; set; }

        public string Banner { get; set; }

        [Display(Name = "Mô tả")]
        [DataType(DataType.Text)]
        public string Description { set; get; }
        
        [Display(Name = "Nội dung")]
        [DataType(DataType.Text)]

        public string Content { get; set; }

        [Display(Name ="Trạng thái")]
        public bool IsStatus { get; set; }

        public ICollection<Product> Products { get; set; }
    }
}
