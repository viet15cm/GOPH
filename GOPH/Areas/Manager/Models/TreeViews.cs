using GOPH.Entites;

namespace GOPH.Areas.Manager.Models
{
    public static class TreeViews
    {

        public static List<CommodityGroup> GetCommodityGroupChierarchicalTree(IEnumerable<CommodityGroup> allCats, string parentId = null)
        {
            return allCats.Where(c => c.ParentCommodityGroupId == parentId)
                            .Select(c => new CommodityGroup()
                            {
                                Id = c.Id,
                                Name = c.Name,
                                ParentCommodityGroupId = c.ParentCommodityGroupId,
                                CommodityGroupChildrens = GetCommodityGroupChierarchicalTree(allCats.ToList(), c.Id)
                            })
                            .ToList();
        }

        public static void CreateTreeViewCommodityGroupSeleteItems(IEnumerable<CommodityGroup> commodityGroups
                                             , List<CommodityGroup> des,
                                              int leve)
        {

            foreach (var commodityGroup in commodityGroups)
            {
                string perfix = string.Concat(Enumerable.Repeat("-", leve));

                des.Add(new CommodityGroup()
                {
                    Id = commodityGroup.Id,
                    Name = perfix + commodityGroup.Name
                });

                if (commodityGroup.CommodityGroupChildrens?.Count > 0)
                {

                    CreateTreeViewCommodityGroupSeleteItems(commodityGroup.CommodityGroupChildrens, des, leve + 1);

                }
            }

        }
    }
}
