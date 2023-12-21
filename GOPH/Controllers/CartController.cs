using GOPH.ATMapper;
using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Models;
using GOPH.Paging;
using GOPH.Services.CartServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.Extensions.Caching.Memory;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using GOPH.Services.CallApiServices;
using GOPH.Security.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace GOPH.Controllers
{
    public class CartController : BaseController
    {

        private ICompositeViewEngine _viewEngine;
        private readonly UserManager<AppUser> _userManager;

        private SignInManager<AppUser> _signInManager;

        public CartController(IMemoryCache cache,
            AppDbContext appDbContext, 
            ILogger<BaseController> logger, 
            IHttpContextAccessor httpContextAccessor,
            ICartServices cartServices, 
            IHttpClientServiceImplementation clientServiceImplementation,
            IAuthorizationService authorizationService,
            ICompositeViewEngine compositeViewEngine,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager) : base(cache, appDbContext, logger, httpContextAccessor, cartServices, clientServiceImplementation, authorizationService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _viewEngine = compositeViewEngine;
        }

        public class ViewCartModel
        {
           
            public IEnumerable<CommodityGroup> Groups { get; set; }

            public List<ProductCart> ProductCarts { get; set; }

            public CustomerCreateDto CustomerCreateDto { get; set; }

            //public ReceiverCreateDto ReceiverCreateDto { get; set; }

            public VouCherBidingDto Voucher { get; set; }

            public Customer Customer { get; set; }

        }

        [BindProperty]
        public ViewCartModel viewCartModel { get; set; }


        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var groups = await GetCommodidtyGroups();

            var IsAu = await _authorizationService.AuthorizeAsync(User, null,
                                                        new CanOptionWholesaleRequirements());
            var list = await _cart.GetJionCartItems(IsAu.Succeeded);

            var model = new ViewCartModel();
            
            model.Groups = groups;
            model.ProductCarts = list;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> OrderInformation()
        {
            var groups = await GetCommodidtyGroups();

            var IsAu = await _authorizationService.AuthorizeAsync(User, null,
                                                    new CanOptionWholesaleRequirements());
            var list = await _cart.GetJionCartItems(IsAu.Succeeded);

            if (list.Count <= 0)
            {
                return RedirectToAction("Index");
            }

            viewCartModel = new ViewCartModel();

            viewCartModel.Groups = groups;

            viewCartModel.ProductCarts = list;

            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);

                viewCartModel.CustomerCreateDto = new CustomerCreateDto();
                viewCartModel.CustomerCreateDto.MobilePhone = user.PhoneNumber;
                viewCartModel.CustomerCreateDto.Name = $"{user.LastName} {user.FirstName}";

                viewCartModel.CustomerCreateDto.NameReceiver = $"{user.LastName} {user.FirstName}"; ;
                viewCartModel.CustomerCreateDto.MobilePhoneReceiver = user.PhoneNumber;
            }

            return View("OrderInformation", viewCartModel);
        }

        [HttpPost]
        public async Task<IActionResult> OrderInformation([FromForm] ViewCartModel viewCartModel)
        {
            var IsAu = await _authorizationService.AuthorizeAsync(User, null,
                                                      new CanOptionWholesaleRequirements());
            
            if (!ModelState.IsValid)
            {
                var groups = await GetCommodidtyGroups();
               
                var list = await _cart.GetJionCartItems(IsAu.Succeeded);
                
                viewCartModel.Groups = groups;
                viewCartModel.ProductCarts = list;

                return View("OrderInformation", viewCartModel);
            }

            var listcart = await _cart.GetJionCartItems(IsAu.Succeeded);

            if (listcart?.Count <= 0)
            {
                return RedirectToAction("index");
            }

            try
            {
                var order = new Order()
                {
                    Id = GetId("DH"),
                    DateCreate = _client.GetNistTime(),
                    

                };

                if (_signInManager.IsSignedIn(User))
                {
                    order.UserId = _userManager.GetUserId(User);
                }
                
                var voucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == viewCartModel.Voucher.Code);

                if (voucher != null && voucher.OrderId == null)
                {
                    voucher.OrderId = order.Id;
                }

                var customer = ObjectMapper.Mapper.Map<Customer>(viewCartModel.CustomerCreateDto);

                if(customer != null)
                {
                    customer.OrderId = order.Id;
                }

                var orderProduct = new List<OrderProduct>();

                foreach (var item in listcart)
                {
                     orderProduct.Add(new OrderProduct()
                    {
                        OderId = order.Id,
                        ProductId = item.Id,
                        Quantity = item.Quantity,
                        

                    });
                }
                await _context.Orders.AddAsync(order);

                await _context.Customers.AddAsync(customer);

                await _context.OrderProducts.AddRangeAsync(orderProduct);
               
                await _context.SaveChangesAsync();


               return RedirectToAction("AlertOrder", new {id= customer.Id});

            }
            catch (Exception)
            {

                return NotFound("Đặt hàng thất bại");
            }
        }

        [HttpGet]
        public async Task<IActionResult> AlertOrder (string id)
        {
            var groups = await GetCommodidtyGroups();

            var customer = await _context.Customers.FindAsync(id);

            if (customer is null)
            {
                return RedirectToAction("index");
            }

            viewCartModel = new ViewCartModel();
            viewCartModel.Groups = groups;

            viewCartModel.Customer = customer; 

            return View(viewCartModel);
        }

        [HttpGet]
        public async Task<IActionResult> Voucher(string code)
        {

            if (!_signInManager.IsSignedIn(User))
            {
              return BadRequest("Ban cần đăng nhập mới thực hiện chức năng này");
            }

            if (string.IsNullOrEmpty(code))
            {
                return BadRequest("Vui lòng nhập mã Vouchers ");
            }
            var voucher = await _context.Vouchers.Include(v => v.Order).FirstOrDefaultAsync(v => v.Code == code);

            if (voucher is null)
            {
                return BadRequest("Mã Vouchers không chính xác");
            }

            if (voucher.OrderId != null)
            {
                return BadRequest("Mã Vouchers đã được sữ dụng");
            }

            var model = new ViewCartModel();

            model.Voucher = ObjectMapper.Mapper.Map<VouCherBidingDto>(voucher);

            model.ProductCarts = await _cart.GetJionCartItems();

            var picePromotion = 0;
           
            foreach (var item in model.ProductCarts)
            {
                picePromotion += ((int)item.TotalPrice * item.Promotion) / 100;
            }

            var totalallpriceCurent = model.ProductCarts.Sum(x => x.TotalPrice);

            var totalallprice = totalallpriceCurent - picePromotion;

            if ((int)totalallprice < model.Voucher.Price)
            {
                return BadRequest("Mã vouchers chỉ áp dụng khi nhỏ hơn hoặc bằng đơn đặt hàng");
            }

            PartialViewResult partialViewResult = PartialView("_OrderInformationPartial", model);

            string viewContent = PartialViewToString.ConvertViewToString(this.ControllerContext, partialViewResult, _viewEngine);

            var alert = new AlertCartVoucher() { IsResult = true, Message = $"Cập nhật Voucher thành công", ReturnHtml = viewContent , Code = voucher.Code , Price = voucher.Price };

            return Json(alert);
        }

        [HttpGet]
        public async Task<IActionResult> AddToCart(string id, int count = 1)
        {
            var product = await _context.Products.Include(x => x.Wholesale).FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return Json(new AlertCart() { IsResult = false, Message = $"Lỗi không thêm được vào giỏ hàng", CountItem = _cart.GetCountItem() });
            }

            var IsAu = await _authorizationService.AuthorizeAsync(User, null,
                                               new CanOptionWholesaleRequirements());

       
            if (!product.IsPrice)
            {

                if (IsAu.Succeeded && product.Wholesale != null)
                {
                    return GetCart(count, product);
                }
               
                return Json(new AlertCart() { IsResult = false, Message = $"Sản phẩm chưa định giá không thêm được vào giỏ hàng", CountItem = _cart.GetCountItem() });

            }

            return GetCart(count, product);
        }

        public JsonResult GetCart(int count , Product product)
        {
            var carts = _cart.GetCartItems();


            var cartitem = carts.Find(p => p.Id == product.Id);

            if (count > 0)
            {
                if (cartitem != null)
                {
                    // Đã tồn tại, tăng thêm 1
                    cartitem.quantity += count;
                    if (cartitem.quantity > 1999)
                    {
                        cartitem.quantity = 2000;
                    }
                }
                else
                {
                    //  Thêm mới
                    if (count > 1999)
                    {
                        count = 2000;
                    }
                    carts.Add(new CartDto() { quantity = count, Id = product.Id });

                }

            }


            // Lưu cart vào Session
            _cart.SaveCartSession(carts);

            var number = _cart.GetCountItem();

            return Json(new AlertCart() { IsResult = true, Message = $"Thêm -{product.Name}- vào giỏ hàng", CountItem = number });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCart(string id)
        {

            var productCarts = _cart.GetCartItems();

            var deleteItem = productCarts.FirstOrDefault(p => p.Id == id);

            if (deleteItem != null)
            {
                productCarts.RemoveAll(p => p.Id == id);
            }

            _cart.SaveCartSession(productCarts);

            var count = _cart.GetCountItem();

            var IsAu = await _authorizationService.AuthorizeAsync(User, null,
                                                    new CanOptionWholesaleRequirements());
            
            var listJion = await _cart.GetJionCartItems(IsAu.Succeeded);


            PartialViewResult partialViewResult = PartialView("_ItemCartPartial", listJion);

            string viewContent = PartialViewToString.ConvertViewToString(this.ControllerContext, partialViewResult, _viewEngine);

            var alert = new AlertCart() { IsResult = true, Message = $"Xóa -{deleteItem.Id}- khỏi giỏ hàng", CountItem = count, ReturnHtml = viewContent };

            return Json(alert);

        }

        [HttpGet]
        public async Task<IActionResult> AddCountCart(string id, int count =1)
        {

            var productCarts = _cart.GetCartItems();

            var curentProduct = productCarts.Find(x => x.Id == id);

            var curentCount = count - curentProduct.quantity;

            if (curentCount > 0)
            {
                curentProduct.quantity = count;

            }
            else if (curentCount < 0)
            {
                if (count <=0)
                {
                    count = 1;
                }
                curentProduct.quantity = count;

            }

            _cart.SaveCartSession(productCarts);

            var countItem = _cart.GetCountItem();

            var IsAu = await _authorizationService.AuthorizeAsync(User, null,
                                                      new CanOptionWholesaleRequirements());

            var productcartjion = await _cart.GetJionCartItems(IsAu.Succeeded);

            PartialViewResult partialViewResult = PartialView("_ItemCartPartial", productcartjion);

            string viewContent = PartialViewToString.ConvertViewToString(this.ControllerContext, partialViewResult, _viewEngine);

            var alert = new AlertCart() { IsResult = true, Message = $"Cập nhật số lượng -{curentProduct}- thành công", CountItem = countItem, ReturnHtml = viewContent };

            return Json(alert);


        }
    }
}
