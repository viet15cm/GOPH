using AutoMapper.Internal.Mappers;
using GOPH.Areas.Manager.Models;
using GOPH.ATMapper;
using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Extensions;
using GOPH.FileManager;
using GOPH.Paging;
using GOPH.Services.CallApiServices;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Caching.Memory;

namespace GOPH.Areas.Manager.Controllers
{

    [Area("manager")]
    public class ProductController : BaseController
    {
        private readonly IFileServices _fileServices;
        public ProductController(IMemoryCache cache,
            AppDbContext appDbContext,
            ILogger<BaseController> logger,
            IHttpClientServiceImplementation httpClientServiceImplementation,
            IFileServices fileServices) : base(cache, appDbContext, logger, httpClientServiceImplementation)
        {
            _fileServices = fileServices;
        }

        [HttpGet]
        public  IActionResult Index([FromQuery] ProductParameters productParameters , [FromRoute]string id , [FromQuery] string search)
        {
            var products = _context.Products;

            if (id != null)
            {
                var productsearchs = products.Where(x => x.Id == id);
               
                return View(PagedList<Product>.ToPagedList(productsearchs, productParameters.PageNumber, productParameters.PageSize));

            }

            if (search != null)
            {
                var productsearchs = products.Where( p => p.Name.ToLower().StartsWith(search.ToLower()));
                
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
            await GetDataSeleteProduct();

            return View("create", new ProductForCreateDto());
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductForCreateDto productForCreateDto)
        {
            if (!ModelState.IsValid)
            {
                await GetDataSeleteProduct();

                return View("create", productForCreateDto);
            }

            var product = ObjectMapper.Mapper.Map<Product>(productForCreateDto);

            product.DateUpdate = DateTime.Now;


            var isduplicateId = await _context.Products.AnyAsync(x => x.Id.Equals(product.Id));

            if (isduplicateId)
            {
                await GetDataSeleteProduct();
                ModelState.AddModelError(string.Empty, $"Lỗi trùng id -{product.Id}-");
                return View("Create", productForCreateDto);
            }

            var isDuplicateCode = await IsCodeDuplicateCreate(product.Code);

            if (!isDuplicateCode)
            {
                await GetDataSeleteProduct();
                ModelState.AddModelError(string.Empty, $"Lỗi trùng mã code -{product.Code}-");
                return View("Create", productForCreateDto);
            }

            try
            {
                product.DateCreate = DateTime.UtcNow;
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                StatusMessage = $"Thêm thành công sản phẩm -{product.Name}-";
                return RedirectToAction("Detail", new {id = product.Id });
            }
            catch (Exception)
            {

                return NotFound("Lỗi Liên hệ admin");
            }


        }


        [HttpGet]
        public async  Task<IActionResult> Update(string id)
        {
            var product = await _context.Products.FindAsync(id);
            
            if (product == null)
            {
                return NotFound($"Không tìm thấy Id {id}");
            }

            await GetDataSeleteProduct();

            var productforupdate = ObjectMapper.Mapper.Map<ProductForUpdateDto>(product);

            return View("Update", productforupdate);


        }

        [NonAction]
        public async Task GetDataSeleteProduct()
        {
            var groups = await GetCommodidtyGroups();

            var des = new List<CommodityGroup>();

            TreeViews.CreateTreeViewCommodityGroupSeleteItems(groups, des, 0);

            ViewData["SeleteCommodityGroups"] = des;

            var conmoditys = await GetCommodidtys();

            ViewData["SeleteCommoditys"] = conmoditys;
        }


        public async Task<bool> IsCodeDuplicateCreate(string code)
        {
            if (code == null)
            {
                return true;
            }

            return !(await _context.Products.AnyAsync(x => x.Code.Equals(code)));
        }

        public async Task<bool> IsIdDuplicateUpdate(string key , string keyUpdate)
        {
            if (key == keyUpdate)
            {
                return true;
            }

            return !await _context.Products.AnyAsync(x => x.Id.Equals(keyUpdate)); 

        }

        public async Task<bool> IsCodeDuplicateUpdate(string key, string keyUpdate)
        {
            if (keyUpdate == null)
            {
                return true;
            }

            if (key == keyUpdate)
            {
                return true;
            }

            return !await _context.Products.AnyAsync(x => x.Id.Equals(keyUpdate));

        }

        [HttpGet]
        public async Task<IActionResult> Detail([FromRoute] string id)
        {
            var product = await _context.Products.
                                Include(x => x.CommodityGroup)
                                .Include(x => x.Commodity)
                                .Include(x => x.Images)
                                .Include(x => x.Wholesale)
                                .FirstOrDefaultAsync(x => x.Id == id);

           return View(product);
        }
        
        [HttpPost]
        public async Task<IActionResult> Update([FromRoute]string id , ProductForUpdateDto productForUpdate)
        {
            if (!ModelState.IsValid)
            {
                await GetDataSeleteProduct();

                return View("Update", productForUpdate) ;
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound($"Không tìm thấy Id {id}");
            }

            var IdDuplicate = await IsIdDuplicateUpdate(product.Id , productForUpdate.Id);

            if (!IdDuplicate)
            {
                 await GetDataSeleteProduct();

                ModelState.AddModelError(string.Empty, $"Lỗi trùng id -{productForUpdate.Id}-");

                return View("Update", productForUpdate);
            }

            var codeDuplicate = await IsCodeDuplicateUpdate(product.Code, productForUpdate.Code);

            if (!codeDuplicate)
            {
                await GetDataSeleteProduct();

                productForUpdate.Code = product.Code;
                ModelState.AddModelError(string.Empty, $"Lỗi trùng Mã vạch -{productForUpdate.Code}-");

                return View("Update", productForUpdate);
            }

            try
            {
                product.Id = productForUpdate.Id;
                product.Name = productForUpdate.Name;
                product.CapitalPrice = productForUpdate.CapitalPrice;
                product.Price = productForUpdate.Price;
                product.Code = productForUpdate.Code;
                product.DisplayName = productForUpdate.DisplayName;
                product.CommodityGroupId = productForUpdate.CommodityGroupId;
                product.IsEvent = bool.Parse(productForUpdate.IsEvent);
                product.CommodidyId  = productForUpdate.CommodidyId;
                product.IsPrice = bool.Parse(productForUpdate.IsPrice);
                product.Hot = bool.Parse(productForUpdate.Hot);
                product.DateUpdate = DateTime.Now;
                product.Promotion = productForUpdate.Promotion;

                _context.Products.Update(product);

                await _context.SaveChangesAsync();

                StatusMessage = $"Cập nhật thành công sản phẩm {product.Name}";

                return RedirectToAction("detail" ,new {id= product.Id});
            }
            catch (Exception)
            {
                return NotFound("Lỗi hệ thống liên hệ admin");
            }

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
        public async Task<IActionResult> SearchIndex([FromForm] string productId , [FromForm] string search)
        {

            if (productId != null)
            {
                return RedirectToAction("index", new { id = productId });
            }

            if (search != null)
            {
                var product = await _context.Products.FindAsync(search);

                if (product != null)
                {
                   return RedirectToAction("index", new { id= product.Id });
                }

            }


            return RedirectToAction("index", new { search= search });
        }


        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }

            return View("delete", product);
            
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id , bool isDelete)
        {
            var product = await _context.Products.Include(x => x.CommodityGroup).Include(x => x.OrderProducts).Include(x => x.Commodity).FirstOrDefaultAsync(x => x.Id == id);

            if (product.Commodity != null || product.OrderProducts.Count > 0 || product.Commodity != null)
            {
                ModelState.AddModelError(string.Empty, "Sản phẩm được xóa không nằm trong mặt hàng, đã được xuất hóa đơn và thuộc nhóm hàng");

                return View("delete", product);
            }
            if (product == null)
            {
                return NotFound("Không tìm thấy sản phẩm");
            }

            try
            {
                _context.Products.Remove(product);

                await _context.SaveChangesAsync();

                StatusMessage = $"Xóa thành công sản phẩm -{product.Name}-";

                return RedirectToAction("index");
            }
            catch (Exception)
            {

                return NotFound("Xóa không thành công liên hệ admin");
            }

        }

        [HttpPost]
        public async Task<IActionResult> IsPrice(string id)
        {
           var product = await _context.Products.Include(x => x.Wholesale).FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return NotFound($"Lỗi không tìm thấy Id {id}");
            }

            try
            {
               
                product.IsPrice = !product.IsPrice;


                await _context.SaveChangesAsync();

                StatusMessage = $"cập nhật hiện giá thành công";

                return RedirectToAction("detail", new { id = product.Id });
            }
            catch (Exception)
            {

                return NotFound($"Lỗi liên hệ admin");
            }

        }

        [HttpPost]
        public async Task<IActionResult> IsHot(string id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound($"Lỗi không tìm thấy Id {id}");
            }

            try
            {
                product.Hot = !product.Hot;

              
                await _context.SaveChangesAsync();
                StatusMessage = $"Cập nhật sản phẩm Hot thành công";

                return RedirectToAction("detail", new { id = product.Id });
            }
            catch (Exception)
            {

                 return NotFound($"Lỗi liên hệ admin");
            }
        }

        [HttpPost]
        public async Task<IActionResult> IsEvent(string id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound($"Lỗi không tìm thấy Id {id}");
            }

            try
            {
                product.IsEvent = !product.IsEvent;
                
              
                await _context.SaveChangesAsync();

                StatusMessage = $"Cập nhật sự kiện thành công";

                return RedirectToAction("detail", new { id = product.Id });
            }
            catch (Exception)
            {

                return NotFound($"Lỗi liên hệ admin");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Contents(string id)
        {
           var product = await _context.Products.FindAsync(id);

            if (product is null)
            {
                return NotFound($"Không tìm thấy Id {id}");
            }

            var productContent = ObjectMapper.Mapper.Map<ProductForContentUpdate>(product);

            return View("contents", productContent);
        }

        [HttpPost]
        public async Task<IActionResult> Contents([FromRoute]string id , [FromForm] ProductForContentUpdate productForContentUpdate)
        {
            var product = await _context.Products.FindAsync(id);

            if(product is null)
            {
                return NotFound($"Không tìm thấy Id {id}");
            }

            try
            {
                product.Description = productForContentUpdate.Description;
                product.Content = productForContentUpdate.Content;

                product.DateUpdate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                StatusMessage = $"Cập nhật nội dung và mô tả thành công";

                return RedirectToAction("contents", new { id = product.Id });
            }
            catch (Exception)
            {

                return NotFound($"Lỗi liên hệ admin");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateImages([FromRoute]string id)
        {
            var product = await _context.Products.Include(p => p.Images).FirstOrDefaultAsync(x => x.Id == id);

            if (product is null)
            {
                return NotFound($"Product for {id}");
            }


            return View("CreateImages", new ImageForCreateDto() { productId = product.Id});
        }

        [HttpPost]
        public async Task<IActionResult> CreateImages([FromForm] ImageForCreateDto imageForCreateDto , [FromRoute] string id)
        {
            if (imageForCreateDto is null)
            {
                return NotFound("không tìm thấy dữ liệu để nạp");
            }


            var product = await _context.Products.FindAsync(id);

            if (product is null)
            {
                return NotFound($"Không tìm thấy Id sản phẩm {id}");
            }

            var FormFile = imageForCreateDto.FormFile;

            if (FormFile == null)
            {
                return NotFound("Lỗi không có file để nạp");
            }

            if (ModelState.IsValid)
            {

                string Url = FileServices.GetUniqueFileName(FormFile.FileName);

                var resultFile = await _fileServices.CreateFileAsync(ProductImg.GetProductImg(), FormFile, Url);


                if (resultFile)
                {
                    var img = new GOPH.Entites.Image();

                    try
                    {
                        img.productId = product.Id;
                        img.Url = Url;

                        await _context.Images.AddAsync(img);

                        await _context.SaveChangesAsync();

                        StatusMessage = $"Thêm Ảnh Thành công";

                        return RedirectToAction("detail", new {id = product.Id });

                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, "Lỗi hệ thống liên hện admin");

                        return View("CreateImages", imageForCreateDto);
                    }

                }

                await _fileServices.DeleteFileAsync(ProductImg.GetProductImg(), Url);

            }
            ModelState.AddModelError(string.Empty, "Thêm ảnh không thành công");

            return View("CreateImages", imageForCreateDto);
        }

        [HttpGet]
        public async Task<IActionResult> EditImg([FromRoute]int id)
        {
            var img = await _context.Images.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (img is null)
            {
                return NotFound($"Lỗi không tìm thấy Id");
            }

            var imgForUpdate = ObjectMapper.Mapper.Map<ImageForUpdateDto>(img);

            return View("EditImg", imgForUpdate);
        }

        [HttpPost]
        public async Task<IActionResult> EditImg([FromForm] ImageForUpdateDto imageForUpdateDto, [FromRoute] int id)
        {
            var img = await _context.Images.Include(x => x.Product).FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (img is null)
            {
                return NotFound($"Lỗi không tìm thấy Id {id}");
            }

            if (imageForUpdateDto is null)
            {
                return NotFound("không tìm thấy dữ liệu để nạp");
            }

            var FormFile = imageForUpdateDto.FormFile;

            if (FormFile == null)
            {
                return NotFound("Lỗi Không có file để nạp");
            }

            if (ModelState.IsValid)
            {
                string olUrl = img.Url;
                
                string Url = FileServices.GetUniqueFileName(FormFile.FileName);

                var resultFile = await _fileServices.CreateFileAsync(ProductImg.GetProductImg(), FormFile, Url);

                if (resultFile)
                {
                    try
                    {
                        img.Url = Url;

                        await _context.SaveChangesAsync();

                        StatusMessage = $"Đổi ảnh Thành công";

                        if (olUrl != null)
                        {
                            await _fileServices.DeleteFileAsync(ProductImg.GetProductImg(), olUrl);
                        }

                        return RedirectToAction("EditImg", new { id = img.Id });

                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, "Lỗi hệ thống liên hện admin");

                        return View("CreateImages", imageForUpdateDto);
                    }

                }

                await _fileServices.DeleteFileAsync(ProductImg.GetProductImg(), Url);

            }
            ModelState.AddModelError(string.Empty, "Đổi ảnh không thành công");

            return View("CreateImages", imageForUpdateDto);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateImporrt()
        {
            var products = await _context.Products.ToListAsync();

            try
            {
                foreach (var product in products)
                {
                    product.DateCreate = DateTime.Now;
                    product.DisplayName = product.Name;

                }

                await _context.SaveChangesAsync();

                StatusMessage = $"Update thành công";

                return RedirectToAction("index");
            }
            catch (Exception e)
            {

                return NotFound($"{e}");
            }

        }

        [HttpGet]

        public IActionResult NewProducts([FromQuery] ProductParameters productParameters )
        {

            var productQuerybles = _context.Products.Where(x => x.DateCreate.AddDays(2) >= DateTime.Now).OrderByDescending(x => x.DateCreate);

            var products = PagedList<Product>.ToPagedList(productQuerybles, productParameters.PageNumber, productParameters.PageSize);

            return View(products);
        }

        [HttpGet]
        public IActionResult UpdateProducts([FromQuery] ProductParameters productParameters)
        {
            var productQuerybles = _context.Products.Where(x => x.DateUpdate.AddDays(2) >= DateTime.Now).OrderByDescending(x => x.DateCreate);

            var products = PagedList<Product>.ToPagedList(productQuerybles, productParameters.PageNumber, productParameters.PageSize);

            return View(products);
        }

    }
}
