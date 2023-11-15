using GOPH.ATMapper;
using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Models;
using GOPH.Paging;
using GOPH.Services.CartServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;


namespace GOPH.Controllers
{
    public class HomeController : BaseController
    {

        private ICompositeViewEngine _viewEngine;
        public HomeController(IMemoryCache cache,
            AppDbContext appDbContext,
            ILogger<BaseController> logger,
            IHttpContextAccessor httpContextAccessor,
            ICartServices cartServices,
            ICompositeViewEngine viewEngine) : base(cache, appDbContext, logger, httpContextAccessor, cartServices)
        {
            _viewEngine = viewEngine;
        }

        public class ViewModel
        {
            public string GroupId { get; set; }

            public List<string> listSerialUrl { get; set; }

            public IEnumerable<CommodityGroup> Groups { get; set; }

            public CommodityGroup CurentGroup { get; set; }

            public PagedList<Product> Products { get; set; }

            public Product CurentProduct { get; set; }
            public string ReturnUrl { get; set; }
          
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {

            var groups = await GetCommodidtyGroups();


            var viewModel = new ViewModel()
            {
                GroupId = null,
                listSerialUrl = new List<string>(),
                Groups = groups.ToList(),
                CurentGroup = null,
                ReturnUrl = Domain()
            };

            var productParameters = new ProductParameters();
            viewModel.Products = PagedList<Product>.ToPagedList(_context.Products.Include(p => p.CommodityGroup).Include(p => p.Commodity), productParameters.PageNumber, productParameters.PageSize);

            return View(viewModel);
        }

        [HttpGet]
        [Route("group/{group?}")]
        public async Task<IActionResult> Group([FromRoute] string group)
        {
            if (group is null)
            {
                return RedirectToAction("index");
            }

            var groups = await GetCommodidtyGroups();

            var curentgroup =  FindGroup(groups.ToList(), group);

            var listGroups = new List<string>() { group };

            if (group != null)
            {
                SerialGroups(curentgroup, listGroups);
            }

            var products = _context.Products.Include(p => p.CommodityGroup).Where(p => listGroups.Contains(p.CommodityGroupId));

            if (curentgroup is null)
            {
                return NotFound($"Không tìm thấy {group}");
            }


            var listSerialUrl = new List<string>();

            FindPostBySlug(groups.ToList(), group, listSerialUrl); // chỉnh lại code

            var viewModel = new ViewModel()
            {
                GroupId = group,
                listSerialUrl = listSerialUrl,
                Groups = groups.ToList(),
                CurentGroup = curentgroup,
                            
                ReturnUrl = Domain()
            };
            
            var productParameters = new ProductParameters();
            viewModel.Products = PagedList<Product>.ToPagedList(products, productParameters.PageNumber, productParameters.PageSize);


            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [HttpGet]
        public async  Task<PartialViewResult> ShowCardProductPartial(int pageNumber, string groupId)
        {
            var productParameters = new ProductParameters() { PageNumber = pageNumber };

            var queryables = _context.Products.Include(p => p.CommodityGroup);

            if (groupId != null)
            {
                var groups = await GetCommodidtyGroups();

                var group = FindGroup(groups.ToList(), groupId);

                var listGroups = new List<string>(){groupId};

                if (group != null)
                {
                    SerialGroups(group, listGroups);
                }

                var queryableAddWhere = queryables.Where(p => listGroups.Contains(p.CommodityGroupId));

                var datas = PagedList<Product>
                    .ToPagedList(queryableAddWhere
                    , productParameters.PageNumber, productParameters.PageSize);

                return PartialView("_CardProducts", datas);
            }


            var products = PagedList<Product>.ToPagedList(queryables, productParameters.PageNumber, productParameters.PageSize);
            
            

            return PartialView("_CardProducts", products);
        }

        [HttpGet]
        public async Task<IActionResult> Detail([FromRoute]string id)
        {
            var product = _context.Products.Include(p => p.CommodityGroup).Include(p => p.Commodity).FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return NotFound("Không tìm thấy dữ liệu ID");
            }

            var groups = await GetCommodidtyGroups();



            var curentgroup = FindGroup(groups.ToList(), product.CommodityGroupId);

            var listGroups = new List<string>() { product.CommodityGroupId };

            if (product.CommodityGroupId != null)
            {
                SerialGroups(curentgroup, listGroups);
            }

            var products = _context.Products.Include(p => p.CommodityGroup).Where(p => listGroups.Contains(p.CommodityGroupId));

            if (curentgroup is null)
            {
                return NotFound($"Không tìm thấy {product.CommodityGroupId}");
            }


            var listSerialUrl = new List<string>();

            FindPostBySlug(groups.ToList(), product.CommodityGroupId, listSerialUrl); // chỉnh lại code

            var viewModel = new ViewModel()
            {
                GroupId = product.CommodityGroup.Id,
                listSerialUrl = listSerialUrl,
                Groups = groups.ToList(),
                CurentGroup = curentgroup,
                CurentProduct = product,
                
                ReturnUrl = Domain()
            };

            var productParameters = new ProductParameters();
            viewModel.Products = PagedList<Product>.ToPagedList(products, productParameters.PageNumber, productParameters.PageSize);

            ViewData["curenturl"] = HttpContextAccessorPathDomainFull();

            return View(viewModel);
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
                                IsPirce = p.IsPrice,
                                Price = p.Price,
                                CapitalPrice = p.CapitalPrice,
                                Promotion = p.Promotion.ToString(),
                                logoUrl = (!string.IsNullOrEmpty(p.UrlImage)) ? p.UrlImage : "/image/product.jpg"
                            }).Take(50).ToList();

          
            return Json(products);
        }


        [HttpPost]
        public IActionResult SearchIndex([FromForm] string productId, [FromForm] string search)
        {
            if (productId is null && string.IsNullOrEmpty(search))
            {
                return NotFound("Không có dữ liệu tìm kiếm");
            }

            if (productId != null)
            {
                return RedirectToAction("detail", new { id = productId });
            }

            if (!string.IsNullOrEmpty(search))
            {
                  return NotFound($"Thuật toán tìm kiếm {search}");
            }


            return NotFound($"Thuật toán tìm kiếm {search}");

        }


        [HttpGet]
        public async Task<IActionResult> AddToCart(string id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return Json(new AlertCart() { IsResult = false, Message = $"Lỗi không thêm được vào giỏ hàng"});
            }
             
       

            var cart = _cart.GetCartItems();
            
            cart.Add(product.Id);

            // Lưu cart vào Session
            _cart.SaveCartSession(cart);

            var count = _cart.GetCountItem();

            return Json(new AlertCart() { IsResult = true, Message = $"Thêm -{product.Name}- vào giỏ hàng", CountItem = count });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCart(string id)
        {
           
            var productCarts = _cart.GetCartItems();

            var product = productCarts.Where(p => p == id).FirstOrDefault();

            if (product != null)
            {
                productCarts.RemoveAll(p => p == id);
            }

            _cart.SaveCartSession(productCarts);

            var count = _cart.GetCountItem();

            PartialViewResult partialViewResult = PartialView("_ItemCartPartial", await _cart.GetJionCartItems());

            string viewContent = PartialViewToString.ConvertViewToString(this.ControllerContext, partialViewResult, _viewEngine);

            var alert = new AlertCart() { IsResult = true, Message = $"Xóa -{product}- khỏi giỏ hàng", CountItem = count, ReturnHtml = viewContent };
            
            return Json(alert);

        }

        [HttpGet]
        public  async Task<IActionResult> AddCountCart(string id, int count)
        {
            var productCarts = _cart.GetCartItems();

            var curentProduct = productCarts.Where(x => x == id).FirstOrDefault();

            var curentProducts = productCarts.Where(x => x == id).ToList();

            var curentCount = count - curentProducts.Count();

            if (curentCount > 0)
            {
                for (int i = 0; i < curentCount; i++)
                {
                    
                    productCarts.Add( curentProduct);
                }
            }
            else if(curentCount < 0)
            {
                var coutf = 0;
              
                for (int i = productCarts.Count - 1; i >= 0; --i)
                {
                    if (productCarts[i] == id)
                    {
                        productCarts.RemoveAt(i);
                        coutf++;
                    }

                    if (coutf == curentCount * (-1))
                    {
                        break;
                    }
                }
                    
            }


             _cart.SaveCartSession(productCarts);

            var countItem = _cart.GetCountItem();
            
            PartialViewResult partialViewResult = PartialView("_ItemCartPartial",await _cart.GetJionCartItems());

            string viewContent = PartialViewToString.ConvertViewToString(this.ControllerContext, partialViewResult, _viewEngine);

            var alert =  new AlertCart() { IsResult = true, Message = $"Cập nhật số lượng -{curentProduct}- thành công", CountItem = countItem , ReturnHtml = viewContent };

            return Json(alert);


        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        



    }
}