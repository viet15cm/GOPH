using GOPH.Areas.Manager.Models;
using GOPH.DbContextLayer;
using GOPH.Entites;
using GOPH.Models;
using GOPH.Services.CartServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace GOPH.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IMemoryCache _cache;
        private readonly IHttpContextAccessor _httpcontext;
        protected readonly AppDbContext _context;
        protected readonly ILogger<BaseController> _logger;
        protected readonly ICartServices _cart;
        public static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);


        [TempData]
        public string StatusMessage { get; set; }
        public BaseController(IMemoryCache cache,
                            AppDbContext appDbContext,
                            ILogger<BaseController> logger, 
                            IHttpContextAccessor httpContextAccessor,
                            ICartServices cartServices)
        {
            _cache = cache;
            _context = appDbContext;
            _logger = logger;
            _httpcontext = httpContextAccessor;
            _cart = cartServices;
        }

        [NonAction]
        public string HttpContextAccessorPathDomainFull()
        {
            return string.Format("{0}://{1}{2}", _httpcontext.HttpContext.Request.Scheme, _httpcontext.HttpContext.Request.Host.ToString(), _httpcontext.HttpContext.Request.Path);
        }

        [NonAction]
        protected string Domain()
        {
           
            return string.Format("{0}://{1}", _httpcontext.HttpContext.Request.Scheme, _httpcontext.HttpContext.Request.Host.ToString());
        }

     

        [NonAction]
        public async Task<IEnumerable<Product>> GetProducts()
        {

            _logger.Log(LogLevel.Information, "Trying to fetch the list of products from cache.");

            if (_cache.TryGetValue(Cache.keyProduct, out IEnumerable<Product> products))
            {
                _logger.Log(LogLevel.Information, "products list found in cache.");
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (_cache.TryGetValue(Cache.keyProduct, out products))
                    {
                        _logger.Log(LogLevel.Information, "products list found in cache.");
                    }
                    else
                    {
                        _logger.Log(LogLevel.Information, "products list not found in cache. Fetching from database.");


                        products = await _context.Products.AsNoTracking().ToListAsync();

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                                // sẽ hết hạn mục nhập nếu nó không được truy cập trong một khoảng thời gian nhất định.
                                .SetSlidingExpiration(TimeSpan.FromDays(1))
                                //sẽ hết hạn mục sau một khoảng thời gian nhất định.
                                .SetAbsoluteExpiration(TimeSpan.FromDays(3))
                                .SetPriority(CacheItemPriority.High);
                        //.SetSize(1024);
                        _cache.Set(Cache.keyProduct, products, cacheEntryOptions);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return products;
        }

        [NonAction]
        public async Task<IEnumerable<CommodityGroup>> GetCommodidtyGroups()
        {

            _logger.Log(LogLevel.Information, "Trying to fetch the list of Groups from cache.");

            if (_cache.TryGetValue(Cache.keyCommodityGroup, out IEnumerable<CommodityGroup> groups))
            {
                _logger.Log(LogLevel.Information, "Groups list found in cache.");
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (_cache.TryGetValue(Cache.keyCommodityGroup, out groups))
                    {
                        _logger.Log(LogLevel.Information, "Groups list found in cache.");
                    }
                    else
                    {
                        _logger.Log(LogLevel.Information, "Groups list not found in cache. Fetching from database.");


                        groups = await _context.CommodityGroups.AsNoTracking().ToListAsync();

                        groups = TreeViews.GetCommodityGroupChierarchicalTree(groups);

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromDays(1))
                                .SetAbsoluteExpiration(TimeSpan.FromDays(3))
                                .SetPriority(CacheItemPriority.High);
                        //.SetSize(1024);
                        _cache.Set(Cache.keyCommodityGroup, groups, cacheEntryOptions);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return groups;
        }

        [NonAction]
        public async Task<IEnumerable<Commodity>> GetCommodidtys()
        {

            _logger.Log(LogLevel.Information, "Trying to fetch the list of Commodidtys from cache.");

            if (_cache.TryGetValue(Cache.keyCommodity, out IEnumerable<Commodity> commoditys))
            {
                _logger.Log(LogLevel.Information, "Commodidtys list found in cache.");
            }
            else
            {
                try
                {
                    await semaphore.WaitAsync();
                    if (_cache.TryGetValue(Cache.keyCommodity, out commoditys))
                    {
                        _logger.Log(LogLevel.Information, "Commodidtys list found in cache.");
                    }
                    else
                    {
                        _logger.Log(LogLevel.Information, "Commodidtys list not found in cache. Fetching from database.");


                        commoditys = await _context.Commodities.AsNoTracking().ToListAsync();

                        var cacheEntryOptions = new MemoryCacheEntryOptions()
                                .SetSlidingExpiration(TimeSpan.FromDays(1))
                                .SetAbsoluteExpiration(TimeSpan.FromDays(3))
                                .SetPriority(CacheItemPriority.Normal);
                        //.SetSize(1024);
                        _cache.Set(Cache.keyCommodity, commoditys, cacheEntryOptions);
                    }
                }
                finally
                {
                    semaphore.Release();
                }
            }
            return commoditys;
        }
        [NonAction]
        protected CommodityGroup FindPostBySlug(List<CommodityGroup> groups, string id, List<string> ids)
        {
            try
            {
                foreach (var p in groups)
                {
                    // xử lý cộng nối tiếp các url có trong node

                    ids.Add(p.Id);

                    if (p.Id == id)
                    {
                        return p;
                    }

                    var p1 = FindPostBySlug(p.CommodityGroupChildrens?.ToList() ?? new List<CommodityGroup>(), id, ids);

                    if (p1 != null)
                        return p1;
                }
                ids.RemoveAt(ids.Count() - 1);

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        [NonAction]
        protected void SerialGroups(CommodityGroup group, List<string> ids)
        {

            if (group?.CommodityGroupChildrens?.Count > 0)
            {
                foreach (var item in group.CommodityGroupChildrens)
                {
                    ids.Add($"{item.Id}");

                    if (item.CommodityGroupChildrens?.Count> 0)
                    {
                        SerialGroups(item, ids);
                    }
                }
            }
            
        }

        [NonAction]

        protected CommodityGroup FindGroup(IEnumerable<CommodityGroup> groups , string id)
        {

            foreach (var item in groups)
            {
                if (item.Id.Equals(id))
                {
                    return item;

                }

                var group = FindGroup(item.CommodityGroupChildrens ?? new List<CommodityGroup>(),id);

                if (group != null) return group;
                
            }

            return null;

           
        }

        public static IEnumerable<SelectListItem> GetEnumSelectList<T>()
        {
            return (Enum.GetValues(typeof(T)).Cast<int>().Select(e => new SelectListItem() { Text = Enum.GetName(typeof(T), e), Value = e.ToString() })).ToList();
        }
    }
}
