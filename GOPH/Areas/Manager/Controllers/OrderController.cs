
using GOPH.ATMapper;
using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Extensions;
using GOPH.Models;
using GOPH.Security.Requirements;
using GOPH.Services.CallApiServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;

namespace GOPH.Areas.Manager.Controllers
{
    [Area("manager")]
    public class OrderController : BaseController
    {

        private readonly IAuthorizationService _authorizationService;

        private readonly SignInManager<AppUser> _signInManager;

        private readonly UserManager<AppUser> _userManager;
        public OrderController(IMemoryCache cache, 
            AppDbContext appDbContext,
            ILogger<BaseController> logger,
            IHttpClientServiceImplementation httpClientServiceImplementation,
            IAuthorizationService authorizationService,
            SignInManager<AppUser> signInManager,
            UserManager<AppUser> userManager
            ) : base(cache, appDbContext, logger,  httpClientServiceImplementation)
        {
            _authorizationService = authorizationService;
            _signInManager = signInManager;
            _userManager    = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.OrderByDescending(x => x.DateCreate)
                .Include(o => o.Invoice)
                .Include(o => o.Voucher)
                .Include(o => o.Customer)
                .Include(o => o.OrderProducts)
                .ThenInclude(o => o.Product)
                .ThenInclude(ob => ob.Wholesale)
                .Include(o => o.Invoice)
                .Include(o => o.AppUser)              
                .Where(o => o.Invoice == null && o.RecycleBin == false)
                .AsNoTracking()
                .ToListAsync();

            foreach (var order in orders)
            {
             
                if (order.AppUser is null)
                {
                    continue;
                   
                }

                var user = await _signInManager.CreateUserPrincipalAsync(order.AppUser);
                var IsAu = await _authorizationService.AuthorizeAsync(user, null,
                                               new CanOptionWholesaleRequirements());

                foreach (var item in order.OrderProducts)
                {
                    if (!IsAu.Succeeded)
                    {
                        continue;
                    }

                    if (item.Product.Wholesale is null)
                    {
                        continue;
                    }

                    item.Product.Price = item.Product.Wholesale.Price;
                    item.Product.Promotion = item.Product.Wholesale.Promotion;

                }
            }

            var orderDetails = ObjectMapper.Mapper.Map<List<OrderDetailDto>>(orders);

            var datetimeNow = DateTime.Now;
            foreach (var order in orderDetails)
            {
                order.ConvertDatetime = ConvertDatetime.GetConvert(datetimeNow, order.DateCreate);
                order.DatetimeDetail = ConvertDatetime.GetDateTime(order.DateCreate);

                order.TotalPrice = order.OrderProducts
                    .Sum(c => (c.Product.Price * c.Quantity) - (c.Product.Price * (c.Product.Promotion * c.Quantity) / 100));

                if (order.Voucher != null)
                {
                    order.TotalPrice -= order.Voucher.Price;
                }
            }

            return View(orderDetails);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(string id)
        {
            var order = await _context.Orders
                .Include(o => o.Invoice)
                .Include(o => o.Voucher)
                .Include(o=> o.Customer)
                .Include(o => o.AppUser)
                .Include(o => o.OrderProducts)
                .ThenInclude(o => o.Product)
                .ThenInclude(ob => ob.Wholesale)
                .FirstOrDefaultAsync(x => x.Id == id);

            var orderDetail = ObjectMapper.Mapper.Map<OrderDetailDto>(order);

            if (orderDetail.AppUser != null)
                {

                    var user = await _signInManager.CreateUserPrincipalAsync(orderDetail.AppUser);
                    var IsAu = await _authorizationService.AuthorizeAsync(user, null,
                                                   new CanOptionWholesaleRequirements());

                    foreach (var item in orderDetail.OrderProducts)
                    {
                        if (!IsAu.Succeeded)
                        {
                            continue;
                        }

                        if (item.Product.Wholesale is null)
                        {
                            continue;
                        }

                        item.Product.Price = item.Product.Wholesale.Price;
                        item.Product.Promotion = item.Product.Wholesale.Promotion;
                        item.IsWholesale = true;
                    }

                }

            var datetimeNow = DateTime.Now;

            orderDetail.ConvertDatetime = ConvertDatetime.GetConvert(datetimeNow, order.DateCreate);
            orderDetail.DatetimeDetail = ConvertDatetime.GetDateTime(order.DateCreate);

            orderDetail.TotalPrice = order.OrderProducts
                .Sum(c => (c.Product.Price * c.Quantity) - (c.Product.Price * (c.Product.Promotion * c.Quantity) / 100));

            if (order.Voucher != null)
            {
                orderDetail.TotalPrice -= order.Voucher.Price;
            }
            return View(orderDetail);
        }

        [HttpPost]
        public async Task<IActionResult> CloseTheOrder([FromRoute]string id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order is null)
            {
                return NotFound($"Không tìm thấy Id đơn hàng {id}");
            }

            try
            {
                order.IsCloseTheOrder = !order.IsCloseTheOrder;

                await _context.SaveChangesAsync();

                if (order.IsCloseTheOrder)
                {
                    StatusMessage = $"Chốt đơn hàng {order.Id} thàng công";
                }
                else
                {
                    StatusMessage = $"Bỏ chốt đơn hàng {order.Id} thàng công";
                }

                return RedirectToAction("detail", new {id = order.Id});
            }
            catch (Exception)
            {
                return NotFound("Lỗi hệ thống thủ lại hoặc liên hệ admin sdt: 0355445775");
            }
        }


        [HttpGet]
        public async Task<IActionResult> RecycleBin()
        {
            var orders = await _context.Orders.OrderByDescending(x => x.DateCreate)
                .Include(o => o.Invoice)
                .Include(o => o.Voucher)
                .Include(o => o.Customer)
                .Include(o => o.OrderProducts)
                .ThenInclude(o => o.Product)
                .ThenInclude(ob => ob.Wholesale)
                .Include(o => o.Invoice)
                .Include(o => o.AppUser)
                .Where(o => o.Invoice == null && o.RecycleBin == true)
                .AsNoTracking()
                .ToListAsync();

            foreach (var order in orders)
            {

                if (order.AppUser is null)
                {
                    continue;

                }

                var user = await _signInManager.CreateUserPrincipalAsync(order.AppUser);
                var IsAu = await _authorizationService.AuthorizeAsync(user, null,
                                               new CanOptionWholesaleRequirements());

                foreach (var item in order.OrderProducts)
                {
                    if (!IsAu.Succeeded)
                    {
                        continue;
                    }

                    if (item.Product.Wholesale is null)
                    {
                        continue;
                    }

                    item.Product.Price = item.Product.Wholesale.Price;
                    item.Product.Promotion = item.Product.Wholesale.Promotion;

                }
            }

            var orderDetails = ObjectMapper.Mapper.Map<List<OrderDetailDto>>(orders);

            var datetimeNow = DateTime.Now;
            foreach (var order in orderDetails)
            {
                order.ConvertDatetime = ConvertDatetime.GetConvert(datetimeNow, order.DateCreate);
                order.DatetimeDetail = ConvertDatetime.GetDateTime(order.DateCreate);

                order.TotalPrice = order.OrderProducts
                    .Sum(c => (c.Product.Price * c.Quantity) - (c.Product.Price * (c.Product.Promotion * c.Quantity) / 100));

                if (order.Voucher != null)
                {
                    order.TotalPrice -= order.Voucher.Price;
                }
            }

            return View(orderDetails);
        }
        [HttpPost]
        public async Task<IActionResult> CancelToRecycleBin(string id)
        {
            var order = await _context.Orders.FirstAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound($"Không tìm thấy id đơn hàng {id}");
            }
                   
            try
            {

                order.RecycleBin = true;

                //_context.Customers.Remove(order.Customer);

                await _context.SaveChangesAsync();

                StatusMessage = $"Bỏ thùng rác thành công đơn hàng {order.Id}";

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                StatusMessage = $"Lỗi không thể bỏ thùng rác  đơn hàng {order.Id}";

                return NotFound(StatusMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Restore (string id)
        {
            var order = await _context.Orders.FirstAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound($"Không tìm thấy id đơn hàng {id}");
            }

            try
            {

                order.RecycleBin = false;

                //_context.Customers.Remove(order.Customer);

                await _context.SaveChangesAsync();

                StatusMessage = $"Khôi phục thành công đơn hàng {order.Id}";

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                StatusMessage = $"Lỗi không thể khôi phục đơn hàng {order.Id}";

                return NotFound(StatusMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var order = await _context.Orders
                .Include(o => o.Invoice)
                .Include(o => o.Voucher)
                .Include(o => o.Customer)
                .Include(o => o.AppUser)              
                .Include(o => o.OrderProducts)
                .ThenInclude(o => o.Product)
                .FirstOrDefaultAsync(x => x.Id == id);
            try
            {
                
                _context.Orders.Remove(order);

                if (order.Invoice != null)
                {
                    _context.Invoices.Remove(order.Invoice);
                }

               //_context.Customers.Remove(order.Customer);

                await _context.SaveChangesAsync();

                StatusMessage = $"Thu hồi thành công đơn hàng {order.Id}";

                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                StatusMessage = $"Lỗi không thể thu hồi đơn hàng {order.Id}";

                return RedirectToAction("Index");
            } 
        }

        [HttpPost]
        public async Task<IActionResult> IssueAnInvoice(string id, bool isResult)
        {
            if (!_signInManager.IsSignedIn(User))
            {
                return NotFound("Đăng nhập mới thực hiện được chức năng này");
            }

            var order = await _context.Orders
                .Include(o => o.Invoice)
                .Include(o => o.Voucher)
                .Include(o => o.Customer)
                .Include(o => o.AppUser)
                .Include(o => o.OrderProducts)
                .ThenInclude(o => o.Product)
                .ThenInclude(ob => ob.Wholesale)
                .FirstOrDefaultAsync(x => x.Id == id);


            if (order.Invoice != null)
            {
                return NotFound("Đơn hàng đã được xuất");
            }

            if (order.AppUser != null)
            {

                var user = await _signInManager.CreateUserPrincipalAsync(order.AppUser);
                var IsAu = await _authorizationService.AuthorizeAsync(user, null,
                                               new CanOptionWholesaleRequirements());

                foreach (var item in order.OrderProducts)
                {
                    if (!IsAu.Succeeded)
                    {
                        item.Price = item.Product.Price;
                        item.Promotion = item.Product.Promotion;
                        item.IsWholesale = false;
                        continue;
                    }

                    if (item.Product.Wholesale is null)
                    {
                        item.Price = item.Product.Price;
                        item.Promotion = item.Product.Promotion;
                        item.IsWholesale = false;
                        continue;
                    }

                    item.Price = item.Product.Wholesale.Price;
                    item.Promotion = item.Product.Wholesale.Promotion;
                    item.IsWholesale = true;
                }

            }
            else
            {
                foreach (var item in order.OrderProducts)
                {
                    item.Price = item.Product.Price;
                    item.Promotion = item.Product.Promotion;
                    item.IsWholesale = false;
                }
            }

            

            var totalPrice = order.OrderProducts
                .Sum(c => (c.Price * c.Quantity) - (c.Price * (c.Promotion * c.Quantity) / 100));


            if (order.Voucher != null)
            {
                totalPrice -= order.Voucher.Price;
            }

            var employee = await _userManager.GetUserAsync(User);


            var invoice = new Invoice();

            invoice.Id = GetId("HD");

            invoice.OrderId = order.Id;

            invoice.EmployeeId = employee.Id;

            invoice.DateCreate = DateTime.Now;
            invoice.TotalPrice = totalPrice;

            try
            {
                
                await _context.Invoices.AddAsync(invoice);
                await _context.SaveChangesAsync();

                StatusMessage = $"Xuất thành công hóa đơn {invoice.Id} ";

                return RedirectToAction("Index");

            }
            catch (Exception)
            {

                StatusMessage = $"Lỗi xuất hóa đơn không thành công";

                return NotFound(StatusMessage);
            }

        }





    }
}
