using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Paging;
using GOPH.Services.CartServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GOPH.Controllers
{
    public class CartController : BaseController
    {
        public CartController(IMemoryCache cache,
            AppDbContext appDbContext, 
            ILogger<BaseController> logger, 
            IHttpContextAccessor httpContextAccessor, 
            ICartServices cartServices) : base(cache, appDbContext, logger, httpContextAccessor, cartServices)
        {
        }

        public class ViewCartModel
        {
           
            public IEnumerable<CommodityGroup> Groups { get; set; }

            public List<ProductCart> ProductCarts { get; set; }
          
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var groups = await GetCommodidtyGroups();
            var list = await _cart.GetJionCartItems();

            var model = new ViewCartModel();
            model.Groups = groups;
            model.ProductCarts = list;
            return View(model);
        }
    }
}
