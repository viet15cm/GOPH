using GOPH.DbContextLayer;
using GOPH.Entites;
using GOPH.Extensions.Extensions;
using GOPH.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Table;
using System.Composition;

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
       
        public  IActionResult Index([FromQuery] ProductParameters productParameters)
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
            
            if (list.Count > 0)
            {
                var exportbytes = ExportFile.ExporttoExcel<Product>(list, reportname);
                
                return File(exportbytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", reportname);
            }
            else
            {
                TempData["Message"] = "Không có dữ liệu để xuất file";
                return View("index");
            }
        }

    }
}
