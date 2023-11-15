namespace GOPH.Dto
{
    public class ProductCart : ProductDto
    {
        public int Quantity { get; set; } = 1;

        public decimal TotalPrice { get; set; }
    }
}
