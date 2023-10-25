using GOPH.Entites;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GOPH.Dto
{
    public class CommodityGroupDto
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string ParentCommodityGroupId { get; set; }


    }
}
