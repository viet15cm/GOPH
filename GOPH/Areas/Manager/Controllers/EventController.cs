using GOPH.ATMapper;
using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.FileManager;
using GOPH.Services.CallApiServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GOPH.Areas.Manager.Controllers
{
    [Area("manager")]
    public class EventController : BaseController
    {

        protected readonly IFileServices _fileServices;
        public EventController(IMemoryCache cache, 
            AppDbContext appDbContext, 
            ILogger<BaseController> logger, 
            IHttpClientServiceImplementation httpClientServiceImplementation,
            IFileServices fileServices
            ) : base(cache, appDbContext, logger, httpClientServiceImplementation)
        {
            _fileServices = fileServices;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var events = _context.Events.ToList();
            return View(events);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("create");
        }


        [HttpPost]
        public  IActionResult Create([FromForm] EventCreateDto eventCreateDto)
        {
            if (eventCreateDto is null)
            {
                return NotFound("Không tìm thấy dữ liệu để nạp");
            }

            if (ModelState.IsValid)
            {
                var item = ObjectMapper.Mapper.Map<Event>(eventCreateDto);

                try
                {
                    _context.Events.Add(item);
                    _context.SaveChanges();

                    StatusMessage = $"Thêm thành công sự kiện {item.Name}";

                    return RedirectToAction("detail" ,new {id= item .Id});
                }
                catch (Exception)
                {

                    return NotFound("Lỗi thử lại hoặc liên hệ admin");
                }
            }

            return View("create", eventCreateDto);

        }

        [HttpGet]
        public IActionResult Detail([FromRoute]string id) 
        {
            var deatilEvent = _context.Events.Include(x => x.Products).FirstOrDefault(x => x.Id == id);

            if (deatilEvent is null)
            {
                return NotFound($"Không tìm thấy dữ liệu {id}");
            }

            return View("detail", deatilEvent);
        }

        [HttpGet]
        public IActionResult Update([FromRoute] string id)
        {
            var item = _context.Events.FirstOrDefault(x => x.Id == id);

            if (item is null)
            {
                return NotFound($"Không tìm thấy dữ liệu {id}");
            }
       
            

            return View("update", item);
        }


        [HttpPost]
        public IActionResult Update([FromForm] EventUpdateDto eventUpdateDto , [FromRoute] string id)
        {
            var item = _context.Events.FirstOrDefault(e => e.Id == id);

            if(item is null)
            {
                return NotFound("Không tìm thấy sự kiện");
            }

            if (eventUpdateDto is null)
            {
               return NotFound("Không tìm thấy dữ liệu sự kiện");
            }


            try
            {
                item.Description = eventUpdateDto.Description;
                item.Name = eventUpdateDto.Name;

                _context.SaveChanges();

                StatusMessage = $"Cập nhật thành công sự kiện {item.Name}";

                return RedirectToAction("detail", new { id = item.Id });
            }
            catch (Exception)
            {

                return NotFound("Không tìm thấy dữ liệu sự kiện");
            }
        }

        [HttpGet]

        public  IActionResult Delete([FromRoute] string id)
        {
            var item = _context.Events.FirstOrDefault(_ => _.Id == id);

            if (item is null)
            {
                return NotFound("Không tim thấy su kiện");
            }

            return View("delete", item);
        }

        [HttpPost]
        public  IActionResult Delete([FromRoute] string id , bool isDelete) 
        {

            var item = _context.Events.Include(x => x.Products).FirstOrDefault(x => x.Id == id);


            if (item is null)
            {
                return NotFound("Không tìm thấy sự kiện");
            }


            try
            {
                if (item.Products?.Count > 0)
                {
                    foreach (var product in item.Products)
                    {
                        product.EventId = null;
                    }
                }

                _context.Events.Remove(item);

                _context.SaveChanges();

                StatusMessage = $"Xóa thành công sự kiện {item.Name}";

                return RedirectToAction("index");
            }
            catch (Exception)
            {


                return NotFound("Lỗi thủ lại hoặc liên hệ admin");
            }
        
        }

        [HttpGet]

        public  IActionResult Contents([FromRoute]string id)
        { 
           var item = _context.Events.FirstOrDefault(x => x.Id == id);

            if (item is null)
            {
                return NotFound("Không tìm thấy sự kiện");
            }

            return View(item);
        
        }


        [HttpPost]
        public IActionResult Contents([FromForm] EventForContentUpdate eventForContentUpdate , [FromRoute]string id)
        {
            var item = _context.Events.FirstOrDefault(x => x.Id == id);

            if(item is null)
            {
                return NotFound("Không tìm thấy sự kiện");
            }

            if (eventForContentUpdate is null)
            {
                return NotFound("Không tìm thấy dữ liệu sự kiện");
            }

            try
            {
                item.Content = eventForContentUpdate.Content;

                _context.SaveChanges();

                StatusMessage = $"Cập nhật thành công nội dung sự kiện {item.Name}";

                return RedirectToAction("contents", new {id= id});
            }
            catch (Exception)
            {
                return NotFound("Lỗi thử lại hoặc liên hệ admin");
            }
        }

        [HttpGet]

        public IActionResult Banner([FromRoute] string id)
        {
            var item = _context.Events.FirstOrDefault(x => x.Id == id);
            if (item is null)
            {
                return NotFound("Không tìm thấy sự kiện");
            }

            return View("Banner", new BannerForCreateDto() { EventId = item.Id , Url = item.Banner});

        }

        [HttpPost]
        public async Task<IActionResult> Banner([FromForm] BannerForCreateDto bannerForCreateDto, [FromRoute] string id)
        {
            var img = await _context.Events.FirstOrDefaultAsync(x => x.Id.Equals(id));

            if (img is null)
            {
                return NotFound($"Lỗi không tìm thấy Id {id}");
            }

            if (bannerForCreateDto is null)
            {
                return NotFound("không tìm thấy dữ liệu để nạp");
            }

            var FormFile = bannerForCreateDto.FormFile;

            if (FormFile == null)
            {
                return NotFound("Lỗi Không có file để nạp");
            }

            if (ModelState.IsValid)
            {
                string olUrl = img.Banner;

                string Url = FileServices.GetUniqueFileName(FormFile.FileName);

                var resultFile = await _fileServices.CreateFileAsync(BannerEvent.GetBannerEvent(), FormFile, Url);

                if (resultFile)
                {
                    try
                    {
                        img.Banner = Url;

                        await _context.SaveChangesAsync();

                        StatusMessage = $"Đổi banner Thành công";

                        if (olUrl != null)
                        {
                            await _fileServices.DeleteFileAsync(BannerEvent.GetBannerEvent(), olUrl);
                        }

                        return RedirectToAction("banner", new { id = img.Id });

                    }
                    catch (Exception)
                    {
                        ModelState.AddModelError(string.Empty, "Lỗi hệ thống liên hện admin");

                        return View("banner", bannerForCreateDto);
                    }

                }

                await _fileServices.DeleteFileAsync(BannerEvent.GetBannerEvent(), Url);

            }
            ModelState.AddModelError(string.Empty, "Đổi banner không thành công");

            return View("banner", bannerForCreateDto);
        }


        [HttpPost]
        public async Task<IActionResult> Status(string id)
        {
            var item = await _context.Events.FindAsync(id);

            if (item == null)
            {
                return NotFound($"Lỗi không tìm thấy Id {id}");
            }

            try
            {
                item.IsStatus = !item.IsStatus;


                await _context.SaveChangesAsync();

                StatusMessage = $"Cập nhật trạng thái thành công";

                return RedirectToAction("detail", new { id = item.Id });
            }
            catch (Exception)
            {

                return NotFound($"Lỗi liên hệ admin");
            }
        }



    }
}
