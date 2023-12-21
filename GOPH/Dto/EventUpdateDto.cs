using System.ComponentModel.DataAnnotations;

namespace GOPH.Dto
{
    public class EventUpdateDto
    {
        [Display(Name = "Tiêu đề")]
        public string Name { get; set; }

        [Display(Name = "Mô tả")]
        [DataType(DataType.Text)]
        [StringLength(1000, ErrorMessage = "{0} không quá {1} từ ")]
        public string Description { get; set; }
    }
    
}
