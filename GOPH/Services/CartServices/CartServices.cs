using GOPH.ATMapper;
using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Models;
using GOPH.Security.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;

namespace GOPH.Services.CartServices
{
    public class CartServices : ICartServices
    {
        private readonly IHttpContextAccessor _httpcontext;
      
        private readonly AppDbContext _context;
        public CartServices(IHttpContextAccessor httpcontext , AppDbContext appDbContext , IAuthorizationService authorizationService)
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

                var listCarts = JsonConvert.DeserializeObject<List<CartDto>>(jsoncart);

                return listCarts.Count();
            }
            return 0;
            
        }

        public async Task<List<ProductCart>> GetJionCartItems(bool iswholesale = false)
        {
            var session = _httpcontext.HttpContext.Session;
            string jsoncart = session.GetString(KeySesstion.CARTKEY);

            var productCarts = new List<ProductCart>();


            if (jsoncart != null)
            {
                var carts = JsonConvert.DeserializeObject<List<CartDto>>(jsoncart);

                foreach ( var item in carts)
                {
                    var product = await _context.Products.Include(x => x.Wholesale).Include(x => x.Images).FirstOrDefaultAsync(x => x.Id.Equals(item.Id));

                    var productCart = ObjectMapper.Mapper.Map<ProductCart>(product);

                    if (productCart != null)
                    {
                        if (iswholesale)
                        {
                            if (product.Wholesale != null)
                            {
                                productCart.Price = product.Wholesale.Price;
                                productCart.Promotion = product.Wholesale.Promotion;
                                productCart.isWholesale = iswholesale;

                            }
                        }

                        productCart.Quantity = item.quantity;
                        productCart.Promotion = product.Promotion;
                        productCart.TotalPrice = item.quantity * productCart.Price;
                    }

                    productCarts.Add(productCart);
                    
                }


                return productCarts;

                //var filteredList = productCarts.GroupBy(e => e.Id).Select(g =>
                //{
                //    var item = g.First();
                //    return new ProductCart
                //    {
                //        Id = item.Id,
                //        Name = item.Name,
                //        Code = item.Code,
                //        TotalPrice = g.Sum(e => e.Price),
                //        Quantity = g.Sum(e => e.Quantity),
                //        Promotion = item.Promotion,
                //        Price = item.Price,
                //        UrlImage = item.UrlImage,
                //        isWholesale = item.isWholesale
                //    };
                //}).ToList();
            }

            return new List<ProductCart>();
        }

        public List<CartDto> GetCartItems()
        {

            var session = _httpcontext.HttpContext.Session;
            string jsoncart = session.GetString(KeySesstion.CARTKEY);

            if (jsoncart != null)
            {
                var listCarts = JsonConvert.DeserializeObject<List<CartDto>>(jsoncart);
                
                return listCarts;
            }
            return new List<CartDto>();
        }

        public void ClearCart()
        {
            var session = _httpcontext.HttpContext.Session;
            session.Remove(KeySesstion.CARTKEY);
        }

        // Lưu Cart (Danh sách CartItem) vào session
        public void SaveCartSession(List<CartDto> ls)
        {
            var session = _httpcontext.HttpContext.Session;
            
            var jsoncart = JsonConvert.SerializeObject(ls);
            
            session.SetString(KeySesstion.CARTKEY, jsoncart);
        }


    }
}
