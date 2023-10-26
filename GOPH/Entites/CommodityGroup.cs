using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GOPH.Entites
{
    public class CommodityGroup
    {

        public CommodityGroup()
        {
            Id = Guid.NewGuid().ToString().Substring(24);
        }

        [Key]
        public string Id { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        public string Name { get; set; }

        public string ParentCommodityGroupId { get; set; }

        [ForeignKey("ParentCommodityGroupId")]
        [Display(Name = "Nhóm cha")]
        public virtual CommodityGroup ParentGroup { set; get; }

   
        public virtual ICollection<CommodityGroup> CommodityGroupChildrens { get; set; }
      
        public virtual ICollection<Product> Products { get; set; }
    }
}
