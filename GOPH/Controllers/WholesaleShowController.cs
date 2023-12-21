using GOPH.DbContextLayer;
using GOPH.Entites;
using GOPH.Paging;
using GOPH.Services.CallApiServices;
using GOPH.Services.CartServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GOPH.Controllers
{
    public class WholesaleShowController : BaseController
    {
        public WholesaleShowController(IMemoryCache cache, AppDbContext appDbContext, ILogger<BaseController> logger, IHttpContextAccessor httpContextAccessor, ICartServices cartServices, IHttpClientServiceImplementation clientServiceImplementation, IAuthorizationService authorizationService) : base(cache, appDbContext, logger, httpContextAccessor, cartServices, clientServiceImplementation, authorizationService)
        {
        }

        public class ViewModel
        {
            public string GroupId { get; set; }

            public List<string> listSerialUrl { get; set; }

            public IEnumerable<CommodityGroup> Groups { get; set; }

            public PagedList<Wholesale> ProductWholesales { get; set; }

            public string ReturnUrl { get; set; }

        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var groups = await GetCommodidtyGroups();

            var viewModel = new ViewModel()
            {
                listSerialUrl = new List<string>(),
                Groups = groups.ToList(),
                ReturnUrl = Domain()
            };

            var wholesaleParameters = new WholesaleParameters();
           
            viewModel.ProductWholesales = PagedList<Wholesale>
                .ToPagedList(_context.Wholesales
                .Include(p => p.Product)
                .ThenInclude(p => p.Commodity)
                , wholesaleParameters.PageNumber, wholesaleParameters.PageSize);

            return View(viewModel);
        }
    }
}
