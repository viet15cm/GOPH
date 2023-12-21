using System.ComponentModel.DataAnnotations;

namespace GOPH.Dto
{
    public class ProductForContentUpdate
    {
        public string Id { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Mô tả")]
        public string Description { set; get; }
        [DataType(DataType.Text)]
        [Display(Name = "Nội dung")]
        public string Content { get; set; }

        public string Name { get; set; }

    }
}
