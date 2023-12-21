using GOPH.ATMapper;
using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Extensions;
using GOPH.Services.CallApiServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace GOPH.Areas.Manager.Controllers
{
    [Area("manager")]
    public class InvoiceController : BaseController
    {

        private readonly UserManager<AppUser> _userManager;

        private readonly SignInManager<AppUser> _signInManager;
        public InvoiceController(IMemoryCache cache,
            AppDbContext appDbContext, 
            ILogger<BaseController> logger,
            IHttpClientServiceImplementation httpClientServiceImplementation,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager
            ) : base(cache, appDbContext, logger, httpClientServiceImplementation)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var invoices = await _context.Invoices.OrderByDescending(x => x.DateCreate)
                .Include(i => i.Order)
                
                .Include(i => i.Employee)
                .AsNoTracking()
                .ToListAsync();

            var invoiceDetails = ObjectMapper.Mapper.Map<List<InvoiceDetailDto>>(invoices);

            var datetimeNow = DateTime.Now;

            foreach (var invoice in invoiceDetails)
            {
                invoice.ConvertDatetime = ConvertDatetime.GetConvert(datetimeNow, invoice.DateCreate);
                invoice.DatetimeDetail = ConvertDatetime.GetDateTime(invoice.DateCreate);

            }

            return View("index", invoiceDetails);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var invoice = await _context.Invoices
               .Include(i => i.Employee)
               .Include(i => i.Order)
               .ThenInclude(i => i.OrderProducts)
               .ThenInclude(i => i.Product)
               .Include(i => i.Order)
               .ThenInclude(i => i.Voucher)
               .Include(i => i.Order)
               .Include(i => i.Order)
               .ThenInclude(i => i.Customer)
               .AsNoTracking()
               .FirstOrDefaultAsync(i => i.Id == id);


            var invoiceDetail = ObjectMapper.Mapper.Map<InvoiceDetailDto>(invoice);

            var datetimeNow = _client.GetNistTime();

            invoiceDetail.ConvertDatetime = ConvertDatetime.GetConvert(datetimeNow, invoiceDetail.DateCreate);
            invoiceDetail.DatetimeDetail = ConvertDatetime.GetDateTime(invoiceDetail.DateCreate);

            
            return View(invoiceDetail);
        }

        [HttpPost]

        public async Task<IActionResult> Recall([FromRoute]string id)
        {
            var invoice = await _context.Invoices
               .Include(i => i.Order)
               .Include(i => i.Order)
               .ThenInclude(i => i.OrderProducts)            
               .FirstOrDefaultAsync(i => i.Id == id);

            if (invoice is null)
            {
                return NotFound($"Không tìm thấy id hóa đơn {id}");
            }
            
           

            try
            {
                invoice.Order.RecycleBin = true;

                foreach (var item in invoice.Order.OrderProducts)
                {
                    item.Price = 0;
                    item.Promotion = 0;
                    item.IsWholesale = false;
                }
                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();


                StatusMessage = $"Thu hồi thành công hóa đơn {invoice.Id} ";

                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                return NotFound($"Lỗi hệ thống liên hện admin 0355445775");
            }

        }





    }
}
