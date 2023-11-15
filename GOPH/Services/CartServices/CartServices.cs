using GOPH.ATMapper;
using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;

namespace GOPH.Services.CartServices
{
    public class CartServices : ICartServices
    {
        private readonly IHttpContextAccessor _httpcontext;

        private readonly AppDbContext _context;
        public CartServices(IHttpContextAccessor httpcontext , AppDbContext appDbContext)
        {

            _httpcontext = httpcontext;
            _context = appDbContext;

        }
        public int GetCountItem()
        {
            var session = _httpcontext.HttpContext.Session;
            string jsoncart = session.GetString(KeySesstion.CARTKEY);

            if (jsoncart != null)
            {

                var listCarts = JsonConvert.DeserializeObject<List<string>>(jsoncart);

                return listCarts.GroupBy(x => x)
                        .Select(x => x.Key)
                        .Count();
            }
            return 0;
            
        }

        public async Task<List<ProductCart>> GetJionCartItems()
        {
            var session = _httpcontext.HttpContext.Session;
            string jsoncart = session.GetString(KeySesstion.CARTKEY);

            var productCarts = new List<ProductCart>();

            if (jsoncart != null)
            {
                var listCarts = JsonConvert.DeserializeObject<List<string>>(jsoncart);

                foreach ( var item in listCarts )
                {
                    var product = await _context.Products.FindAsync(item);
                    var productCart = ObjectMapper.Mapper.Map<ProductCart>(product);
                    if (product != null)
                    {
                        productCarts.Add(productCart);
                    }


                }

                var filteredList = productCarts.GroupBy(e => e.Id).Select(g =>
                {
                    var item = g.First();
                    return new ProductCart
                    {
                        Id = item.Id,
                        Name = item.Name,
                        Code = item.Code,
                        TotalPrice = g.Sum(e => e.Price),
                        Quantity = g.Sum(e => e.Quantity),
                        Promotion = item.Promotion,
                        Price = item.Price,
                        UrlImage = item.UrlImage
                    };
                }).ToList();

                return filteredList;
            }
            return new List<ProductCart>();
        }

        public List<string> GetCartItems()
        {

            var session = _httpcontext.HttpContext.Session;
            string jsoncart = session.GetString(KeySesstion.CARTKEY);

            if (jsoncart != null)
            {
                var listCarts = JsonConvert.DeserializeObject<List<string>>(jsoncart);
                
                return listCarts;
            }
            return new List<string>();
        }

        public void ClearCart()
        {
            var session = _httpcontext.HttpContext.Session;
            session.Remove(KeySesstion.CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
        public void SaveCartSession(List<string> ls)
        {
            var session = _httpcontext.HttpContext.Session;
            string jsoncart = JsonConvert.SerializeObject(ls);
            session.SetString(KeySesstion.CARTKEY, jsoncart);
        }


    }
}
