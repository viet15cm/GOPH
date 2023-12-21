using GOPH.Entites;

namespace GOPH.Dto
{
    public class ProductDetailDto : Product
    {
        public bool isWholesale { get; set; }
    }
}
