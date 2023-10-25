using AutoMapper.Internal.Mappers;
using GOPH.Areas.Manager.Models;
using GOPH.ATMapper;
using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Extensions.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GOPH.Areas.Manager.Controllers
{
    [Area("manager")]
    public class CommodityGroupController : Controller
    {
        private readonly AppDbContext _context;

        public CommodityGroupController (AppDbContext appDbContext)
        {
            _context = appDbContext;
        }


        [TempData]
        public string StatusMessage { get; set; }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
           

            var groups = await _context.CommodityGroups.ToListAsync();

            groups = TreeViews.GetCommodityGroupChierarchicalTree(groups);

            return View(groups);

        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {

            var groups = await _context.CommodityGroups.ToListAsync();

            groups = TreeViews.GetCommodityGroupChierarchicalTree(groups);

            var des = new List<CommodityGroup>();

            TreeViews.CreateTreeViewCommodityGroupSeleteItems(groups, des, 0);

            ViewData["SeleteCommodityGroups"] = des;

            return View("create", new CommodityGroup());
        }

        public async Task<IActionResult> Create([FromForm] CommodityGroup commodityGroup)
        {
            if (!ModelState.IsValid)
            {
                return View("index");
            }

            try
            {
                await _context.CommodityGroups.AddAsync(commodityGroup);
                await _context.SaveChangesAsync();

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
            var group = await _context.CommodityGroups.FindAsync(id);

            if (group == null)
            {
                return NotFound($"id {id} nhóm hàng không tồn tại");
            }

            return View(group);


        }
        [HttpPost]
        public async Task<IActionResult> Delete([FromRoute] string id , bool isDelete)
        {
            var group = await _context.CommodityGroups.Include(c => c.CommodityGroupChildrens).Include(c => c.ParentGroup).FirstOrDefaultAsync(x => x.Id == id);

            if (group == null)
            {
                return NotFound($"id {id} nhóm hàng không tồn tại");
            }

            if (group.CommodityGroupChildrens?.Count > 0)
            {
                foreach (var item in group.CommodityGroupChildrens)
                {
                    item.ParentCommodityGroupId = group.ParentCommodityGroupId;
                }
            }

            try
            {
                _context.CommodityGroups.Remove(group);
                await _context.SaveChangesAsync();

                StatusMessage = "Xóa thành công .";

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
            var group = await _context.CommodityGroups.FindAsync(id);

            if (group == null)
            {
                return NotFound($"id {id} nhóm hàng không tồn tại");
            }

            var groups = await _context.CommodityGroups.ToListAsync();

            groups = TreeViews.GetCommodityGroupChierarchicalTree(groups);

            var des = new List<CommodityGroup>();

            TreeViews.CreateTreeViewCommodityGroupSeleteItems(groups, des, 0);

            ViewData["SeleteCommodityGroups"] = des;

            return View("Edit", group);
        }

        public async Task<IActionResult> Edit([FromRoute] string id , [FromForm] CommodityGroup commodityGroup)
        {
            var group = await _context.CommodityGroups.Include(c => c.CommodityGroupChildrens).Include(c => c.ParentGroup).FirstOrDefaultAsync(c => c.Id == id);

            if (group == null)
            {
                return NotFound($"id {id} nhóm hàng không tồn tại");
            }

        
            if (commodityGroup != null)
            {
                if (group.Id == commodityGroup.ParentCommodityGroupId)
                {
                    return NotFound("Lỗi trùng lặp id");
                }
            }



            if (group.CommodityGroupChildrens?.Count > 0)
            {
                if (group.ParentCommodityGroupId != commodityGroup.ParentCommodityGroupId)
                {
                    foreach (var item in group.CommodityGroupChildrens)
                    {
                        item.ParentCommodityGroupId = group.ParentCommodityGroupId;
                    }
                }
            }


            try
            {
                group.ParentCommodityGroupId = commodityGroup.ParentCommodityGroupId;

                group.Name = commodityGroup.Name;

                _context.CommodityGroups.Update(group);

               await _context.SaveChangesAsync();
                StatusMessage = "Chỉnh sửa thành công .";

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
            string reportname = $"nhom_hang_{Guid.NewGuid():N}.xlsx";

            var list = await _context.CommodityGroups.ToListAsync();

            var listDto = ObjectMapper.Mapper.Map<List<CommodityGroupDto>>(list);


            if (list.Count > 0)
            {
                var exportbytes = ExportFile.ExporttoExcel<CommodityGroupDto>(listDto, reportname);

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
