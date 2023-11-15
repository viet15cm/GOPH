using GOPH.Dto;
using GOPH.Entites;

namespace GOPH.Services.CartServices
{
    public interface ICartServices
    {
        public int GetCountItem();
        public List<string> GetCartItems();

        public Task<List<ProductCart>> GetJionCartItems();
        public void ClearCart();
        public void SaveCartSession(List<string> ls);
    }
}
