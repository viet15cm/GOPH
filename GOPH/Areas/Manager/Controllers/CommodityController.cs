using AutoMapper.Internal.Mappers;
using GOPH.ATMapper;
using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GOPH.Areas.Manager.Controllers
{
    [Area("manager")]
    public class CommodityController : Controller
    {
        private readonly AppDbContext _context;

        public CommodityController(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }


        [TempData]
        public string StatusMessage { get; set; }


        [HttpGet]
        public  IActionResult Index()
        {

            var commoditys =  _context.Commodities.ToList();
            return View(commoditys);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new Commodity());
        }

        [HttpPost]
        public IActionResult Create([FromForm] Commodity commodity) 
        {
            if (!ModelState.IsValid)
            {
                return View("create");
            }

            try
            {
                var result = _context.Commodities.Add(commodity);

                _context.SaveChanges();

                StatusMessage = "Thêm thành công .";

                return RedirectToAction("index");
            }

            catch (Exception)
            {
                return NotFound("Lỗi thử lại hoặc liên hệ admin");
            }
           
         
        }

        [HttpGet]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var commodity =  await _context.Commodities.FindAsync(id);

            if (commodity is null)
            {
                return NotFound($"Id {id} hàng hóa không tồn tại");

            }

            return View("delete", commodity);
        }

        [HttpPost]

        public async Task<IActionResult> Delete([FromRoute] string id , bool isDelete)
        {
            var commodity = await _context.Commodities.FindAsync(id);

            if (commodity is null)
            {
                return NotFound($"Id {id} hàng hóa không tồn tại");

            }

            try
            {
                _context.Commodities.Remove(commodity);

                await _context.SaveChangesAsync();

                StatusMessage = "Xóa thành công";

                return RedirectToAction("index");
            }
            catch (Exception)
            {
                return NotFound("Lỗi thử lại hoặc liên hệ admin");
            }

        }

        [HttpGet]
        
        public async Task<IActionResult> Edit([FromRoute] string id)
        {
            var commodity = await _context.Commodities.FindAsync(id);

            if (commodity is null)
            {
                return NotFound($"Id {id} hàng hóa không tồn tại");

            }

            return View("edit", commodity);
        }

        [HttpPost]

        public async Task<IActionResult> Edit([FromRoute] string id , [FromForm] Commodity commodity)
        {

            var cmd = await _context.Commodities.FindAsync(id);
            

            if (cmd is null)
            {
                return NotFound($"Id {id} hàng hóa không tồn tại");

            }

            try
            {
                cmd.Name = commodity.Name;

                _context.Commodities.Update(cmd);
                await _context.SaveChangesAsync();

                StatusMessage = "Chỉnh sửa thành công";

                return RedirectToAction("index");
            }
            catch (Exception)
            {


                return NotFound("Lỗi thử lại hoặc liên hệ admin");
            }

        }


        [HttpPost]
        public async Task<IActionResult> DownloadReport(IFormCollection obj)
        {

            string reportname = $"hang_hoa_{Guid.NewGuid():N}.xlsx";

            var list = await _context.Commodities.ToListAsync();

            var listDto = ObjectMapper.Mapper.Map<List<CommodityDto>>(list);


            if (list.Count > 0)
            {
                var exportbytes = ExportFile.ExporttoExcel<CommodityDto>(listDto, reportname);

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
