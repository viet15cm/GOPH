using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Services.CallApiServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GOPH.Areas.Manager.Controllers
{
    [Area("manager")]
    public class WholesaleController : BaseController
    {
        public WholesaleController(IMemoryCache cache, AppDbContext appDbContext, ILogger<BaseController> logger, IHttpClientServiceImplementation httpClientServiceImplementation) : base(cache, appDbContext, logger, httpClientServiceImplementation)
        {
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var wholesale = await _context.Wholesales.Include(x => x.Product).ToListAsync();

            return View(wholesale);
        }


        [HttpGet]
        public async Task<IActionResult> Create([FromRoute]string id)
        {
           var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound($"Không tìm thấy Id {id}");
            }

            return View("Create", new Wholesale() { ProductId = id , Product = product });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm]WholesaleForCreateDto wholesaleForCreateDto , [FromRoute] string id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound($"Không tìm thấy Id {id}");
            }

            if (!ModelState.IsValid)
            {

                return View("Create", new Wholesale() { ProductId = id, Product = product });
            }

            if (wholesaleForCreateDto.Price >= product.Price || wholesaleForCreateDto.Price <= 0)
            {
                ModelState.AddModelError(string.Empty, $"Giá để sỉ không thể bằng 0 hoặc lớn hơn hoặc bằng giá bán");

                return View("Create", new Wholesale() { ProductId = id, Product = product });
            }

            var wholesale = new Wholesale();

            try
            {
           
                
                wholesale.ProductId = product.Id;
                wholesale.Price = wholesaleForCreateDto.Price;
                wholesale.Promotion = wholesaleForCreateDto.Promotion;

                await _context.Wholesales.AddAsync(wholesale);
                     
                await _context.SaveChangesAsync();

                StatusMessage = $"Thêm thành công giá sỉ";

                return RedirectToAction("detail", "product", new { id = product.Id });
            }
            catch (Exception)
            {

              return NotFound($"Lỗi liên hệ admin");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Update(string id)
        {
            var wholesale = await _context.Wholesales.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);

            if (wholesale is null)
            {
                return NotFound($"Không tìm thấy Id {id}");

            }

            return View("Update", wholesale);
        }

        [HttpPost]
        
        public async Task<IActionResult> Update([FromForm]WholesaleForUpdateDto wholesaleForUpdateDto , string id)
        {
            var wholesale = await _context.Wholesales.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);

            if (wholesale is null)
            {
                return NotFound($"Không tìm thấy Id {id}");
            }

            if (wholesaleForUpdateDto.Price >= wholesale.Product.Price || wholesaleForUpdateDto.Price <=0)
            {
                ModelState.AddModelError(string.Empty, $"Giá để sỉ không thể bằng 0 hoặc lớn hơn hoặc bằng giá bán");

                return View("Update", wholesale);
            }

            try
            {
                wholesale.Price = wholesaleForUpdateDto.Price;
                wholesale.Promotion = wholesaleForUpdateDto.Promotion;

                await _context.SaveChangesAsync();

                StatusMessage = $"Cập nhật thành công giá sỉ";

                return RedirectToAction("detail", "product", new { id = wholesale.ProductId });
            }
            catch (Exception)
            {
                return NotFound($"Lỗi liên hệ admin");
            }
        }

        [HttpGet]

        public async Task<IActionResult> Delete(string id)
        {
            var wholesale = await _context.Wholesales.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);

            if (wholesale is null)
            {
                return NotFound($"Không tìm thấy Id {id}");
            }

            return View("delete", wholesale);

        }

        [HttpPost]

        public async Task<IActionResult> Delete([FromRoute]string id, bool isDetele)
        {
            var wholesale = await _context.Wholesales.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id == id);

            if (wholesale is null)
            {
                return NotFound($"Không tìm thấy Id {id}");
            }

            try
            {
                _context.Wholesales.Remove(wholesale);
                await _context.SaveChangesAsync();

                StatusMessage = $"Xóa thành công giá sỉ";

                return RedirectToAction("detail", "product" ,new { id = wholesale.ProductId });
            }
            catch (Exception)
            {

                return NotFound($"Không tìm thấy Id {id}");
            }


        }
    }
}
