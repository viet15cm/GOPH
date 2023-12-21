using System.ComponentModel.DataAnnotations;

namespace GOPH.Dto
{
    public class EventForContentUpdate
    {
        
        [Display(Name = "Nội dung")]
        [DataType(DataType.Text)]
        public string Content { get; set; }
    }
}
