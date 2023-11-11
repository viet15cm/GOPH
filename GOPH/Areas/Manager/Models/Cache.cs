using System.Collections.Generic;

namespace GOPH.Areas.Manager.Models
{
    public class Cache
    {
        public static string keyProduct = "_product";
        public static string keyCommodityGroup = "_group";
        public static string keyCommodity = "_commodity";
        public Dictionary<string, string> ObjCache { get; set; }
        public Cache()
        {
            ObjCache = new Dictionary<string, string>();
            ObjCache.Add("_product", "Sản Phẩm");
            ObjCache.Add("_group", "Nhóm Sản Phẩm");
            ObjCache.Add("_commodity", "Hàng Hóa");
        }

    }
}
