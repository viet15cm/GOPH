using GOPH.Dto;
using GOPH.Entites;

namespace GOPH.Services.CartServices
{
    public interface ICartServices
    {
        public int GetCountItem();
        public List<CartDto> GetCartItems();

        public Task<List<ProductCart>> GetJionCartItems(bool iswholesale = false);
        public void ClearCart();
        public void SaveCartSession(List<CartDto> ls);
    }
}
