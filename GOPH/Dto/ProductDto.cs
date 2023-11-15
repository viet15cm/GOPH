using GOPH.Entites;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GOPH.Dto
{
    public class ProductDto
    {
        [Key]
        public string Id { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }


        public decimal Price { get; set; }

        public int Promotion { get; set; }

        public string Description { get; set; }

        public decimal CapitalPrice { get; set; }

        public string UrlImage { get; set; }
        public bool Hot { get; set; }

        [Display(Name = "Loại")]
        public string CommodidyId { get; set; }

        [Display(Name = "Hiện giá")]
        public bool IsPrice { get; set; }

        [Display(Name = "Nhóm")]
        public string CommodityGroupId { get; set; }








        }
}
