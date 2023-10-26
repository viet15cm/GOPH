using AutoMapper.Internal.Mappers;
using GOPH.ATMapper;
using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Extensions.Extensions;
using GOPH.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Composition;
using System.Linq;

namespace GOPH.Areas.Manager.Controllers
{

    [Area("manager")]
    public class ProductController : Controller
    {
        private readonly AppDbContext _context;

        public ProductController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }


        [TempData]
        public string StatusMessage { get; set; }
       
        public   IActionResult Index([FromQuery] ProductParameters productParameters)
        {

            var product = _context.Products;

            var productPaging = PagedList<Product>.ToPagedList(product, productParameters.PageNumber, productParameters.PageSize);

            return View(productPaging);

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

    }
}
