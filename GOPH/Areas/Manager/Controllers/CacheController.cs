using GOPH.Areas.Manager.Models;
using GOPH.DbContextLayer;
using GOPH.Services.CallApiServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GOPH.Areas.Manager.Controllers
{
    [Area("manager")]
    public class CacheController : BaseController
    {
        public CacheController(IMemoryCache cache, AppDbContext appDbContext, ILogger<BaseController> logger, IHttpClientServiceImplementation httpClientServiceImplementation) : base(cache, appDbContext, logger, httpClientServiceImplementation)
        {
        }

        public IActionResult Index()
        {
            return View(new Cache());
        }

        [HttpPost]
        public async Task<IActionResult> Update()
        {        
            try
            {
                await UpdateCache();
                StatusMessage = $"Cập nhật cache thành công";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                StatusMessage = $"Cập nhật không thành công";
                return View("Index");
            }
           
        }
    }
}
