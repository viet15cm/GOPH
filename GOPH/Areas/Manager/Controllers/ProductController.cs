using AutoMapper.Internal.Mappers;
using GOPH.Areas.Manager.Models;
using GOPH.ATMapper;
using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Extensions.Extensions;
using GOPH.Paging;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Caching.Memory;


namespace GOPH.Areas.Manager.Controllers
{

    [Area("manager")]
    public class ProductController : BaseController
    {
        public ProductController(IMemoryCache cache, AppDbContext appDbContext, ILogger<BaseController> logger) : base(cache, appDbContext, logger)
        {
        }

        public   IActionResult Index([FromQuery] ProductParameters productParameters , [FromRoute]string id)
        {
            var products = _context.Products;

            if (id != null)
            {
               var productsearchs = products.Where(x => x.Id == id);
               
                return View(PagedList<Product>.ToPagedList(productsearchs, productParameters.PageNumber, productParameters.PageSize));

            }
            return View(PagedList<Product>.ToPagedList(products, productParameters.PageNumber, productParameters.PageSize));

        }

        [HttpPost]
        public async Task<IActionResult> DownloadReport(IFormCollection obj)
        {
            string reportname = $"san_pham_{Guid.NewGuid():N}.xlsx";

            var list = await _context.Products.ToListAsync();

            var listDto = ObjectMapper.Mapper.Map<List<ProductDto>>(list);

            if (list.Count > 0)
            {
                var exportbytes = ExportFile.ExporttoExcel<ProductDto>(listDto, reportname);
                
                return File(exportbytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
            }
            else
            {
                TempData["Message"] = "Không có dữ liệu để xuất file";
                return View("index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditAllProduct()
        {
            var commodity = await _context.Commodities.ToListAsync();

            var commodityGroups = await _context.CommodityGroups.ToListAsync();


            var products = await (from p in _context.Products
                                  select p).ToListAsync();

            try
            {

                foreach (var product in products)
                {
                    var update = await _context.Products.FindAsync(product.Id);

                    var array = product.NhomHang.Split('>').ToArray();

                    product.NhomHang = array[array.Length - 1];

                    update.CommodidyId = commodity.Where(c => c.Name.Contains(product.HangHoa)).Select(c => c.Id).FirstOrDefault();
                    update.CommodityGroupId = commodityGroups.Where(c => product.NhomHang.Contains(c.Name)).Select(c => c.Id).FirstOrDefault();
                }

                await _context.SaveChangesAsync();

                StatusMessage = $"Cập nhật thành công danh sách";

                return RedirectToAction("Index");
            }
            catch (Exception e)
            {
                return NotFound(e);
            }

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var groups = await GetCommodidtyGroups();

            var des = new List<CommodityGroup>();

            TreeViews.CreateTreeViewCommodityGroupSeleteItems(groups, des, 0);

            ViewData["SeleteCommodityGroups"] = des;

            var conmoditys = await GetCommodidtys();

            ViewData["SeleteCommoditys"] = conmoditys;
            return View("create", new ProductForCreateDto());
        }

        [HttpGet]
        public async  Task<IActionResult> Update(string id)
        {
            var product = await _context.Products.FindAsync(id);
            
            if (product == null)
            {
                return NotFound($"Không tìm thấy Id {id}");
            }

            var groups = await GetCommodidtyGroups();

            var des = new List<CommodityGroup>();

            TreeViews.CreateTreeViewCommodityGroupSeleteItems(groups, des, 0);

            ViewData["SeleteCommodityGroups"] = des;

            var conmoditys = await GetCommodidtys();

            ViewData["SeleteCommoditys"] = conmoditys;

            return View("Update", product);


        }
        [HttpPost]
        public async Task<IActionResult> Update([FromRoute]string id , ProductForUpdateDto productForUpdate)
        {
            if (!ModelState.IsValid)
            {
                var groups = await GetCommodidtyGroups();

                var des = new List<CommodityGroup>();

                TreeViews.CreateTreeViewCommodityGroupSeleteItems(groups, des, 0);

                ViewData["SeleteCommodityGroups"] = des;

                var conmoditys = await GetCommodidtys();

                ViewData["SeleteCommoditys"] = conmoditys;

                return View("Update",ObjectMapper.Mapper.Map<Product>(productForUpdate)) ;
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound($"Không tìm thấy Id {id}");
            }

            try
            {
                //Type TypeProductForupdateDto = productForUpdate.GetType();

                //Type TypeProduct = product.GetType();

                //IList<PropertyInfo> props = new List<PropertyInfo>(TypeProductForupdateDto.GetProperties());

                //foreach (PropertyInfo prop in props)
                //{
                    
                //    PropertyInfo propertyInfo = TypeProduct.GetProperty(prop.Name);
                //    propertyInfo.SetValue(product, prop.GetValue(productForUpdate, null));
                //}


                product.Name = productForUpdate.Name;
                product.CapitalPrice = productForUpdate.CapitalPrice;
                product.Price = productForUpdate.Price;
                product.Code = productForUpdate.Code;
                product.CommodityGroupId = productForUpdate.CommodityGroupId;

                product.CommodidyId  = productForUpdate.CommodidyId;
                product.IsPrice = bool.Parse(productForUpdate.IsPrice);
                product.Hot = bool.Parse(productForUpdate.Hot);

                product.Promotion = productForUpdate.Promotion;

                _context.Products.Update(product);

                await _context.SaveChangesAsync();

                StatusMessage = $"Cập nhật thành công sản phẩm {product.Name}";

                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return NotFound("Lỗi Liên hệ admin");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductForCreateDto productForCreateDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var product = ObjectMapper.Mapper.Map<Product>(productForCreateDto);
                  
                    await _context.Products.AddAsync(product);
                    await _context.SaveChangesAsync();
                    StatusMessage = $"Thêm thành công sản phẩm{product.Name}";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {

                    return NotFound("Lỗi Liên hệ admin");
                }
               
            }

            var groups = await GetCommodidtyGroups();

            var des = new List<CommodityGroup>();

            TreeViews.CreateTreeViewCommodityGroupSeleteItems(groups, des, 0);

            ViewData["SeleteCommodityGroups"] = des;

            var conmoditys = await GetCommodidtys();

            ViewData["SeleteCommoditys"] = conmoditys;
            
            return View("create", productForCreateDto);

        }

        [HttpPost]
        public async Task<JsonResult> Search(string prefix)
        {
            var queryProduct = (await GetProducts()).AsQueryable();

            var products = (from p in queryProduct
                            where p.Name.ToLower().StartsWith(prefix.ToLower())
                            select new
                            {
                                val = p.Id,
                                label = p.Name,
                                price = p.Price,
                                discount = p.CapitalPrice,
                                logoUrl = (!string.IsNullOrEmpty(p.UrlImage)) ? p.UrlImage : "/image/product.jpg"
                            }).Take(50).ToList();

            return Json(products);
        }

        [HttpPost]
        public IActionResult SearchIndex([FromForm] string productId)
        {

            return RedirectToAction("index", new { id = productId });
        }

       

    }
}
