using GOPH.Entites;

namespace GOPH.Dto
{
    public class ProductCart : Product
    {
        public int Quantity { get; set; } = 1;

        public decimal TotalPrice { get; set; }

        public bool isWholesale { get; set; }
    }
}
