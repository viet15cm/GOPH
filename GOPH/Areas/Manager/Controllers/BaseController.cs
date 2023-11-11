using GOPH.Areas.Manager.Models;
using GOPH.DbContextLayer;
using GOPH.Entites;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Threading;

namespace GOPH.Areas.Manager.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IMemoryCache _cache;

        protected readonly AppDbContext _context;
        protected readonly ILogger<BaseController> _logger;
        public static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1, 1);

        [TempData]
        public string StatusMessage { get; set; }
        public BaseController(IMemoryCache cache , AppDbContext appDbContext, ILogger<BaseController> logger)
        {
            _cache = cache;
            _context = appDbContext;
            _logger = logger;

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
        public async Task UpdateCache()
        {

            var task = new Task(() =>
            {
                _cache.Remove(Cache.keyProduct);
                _cache.Remove(Cache.keyCommodityGroup);
                _cache.Remove(Cache.keyCommodity);

            });
            
            task.Start();
            await task;
            await GetProducts();
            await GetCommodidtyGroups();
            await GetCommodidtys();
        }

    }
}
