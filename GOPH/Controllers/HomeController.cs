using GOPH.ATMapper;
using GOPH.DbContextLayer;
using GOPH.Dto;
using GOPH.Entites;
using GOPH.Extensions;
using GOPH.Models;
using GOPH.Paging;
using GOPH.Security.Requirements;
using GOPH.Services.CallApiServices;
using GOPH.Services.CartServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;


namespace GOPH.Controllers
{
    public class HomeController : BaseController
    {

        private readonly ICompositeViewEngine _viewEngine;

       
        public HomeController(IMemoryCache cache,
            AppDbContext appDbContext,
            ILogger<BaseController> logger,
            IHttpContextAccessor httpContextAccessor,
            ICartServices cartServices,
            IHttpClientServiceImplementation clientServiceImplementation,
            ICompositeViewEngine viewEngine,
            IAuthorizationService authorizationService) : base(cache, appDbContext, logger, httpContextAccessor, cartServices, clientServiceImplementation, authorizationService)
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

            public PagedList<Product> ProductHots { get; set; }
            
            public ProductDetailDto ProductDetailDto { get; set; }

            public PagedList<Product> ProductPromotions { get; set; }

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
            productParameters.PageSize = 8;
            viewModel.ProductHots = PagedList<Product>
                .ToPagedList(_context.Products.Include(p => p.CommodityGroup)
                .Include(p => p.Commodity).Include(x => x.Images)
                .Where(x => x.Hot == true), productParameters.PageNumber, productParameters.PageSize);

            viewModel.ProductPromotions = PagedList<Product>
                .ToPagedList(_context.Products.Include(p => p.CommodityGroup).Include(x => x.Images)
                .Include(p => p.Commodity).Where(x => x.IsEvent == true), productParameters.PageNumber, productParameters.PageSize);


         
            var count = PaginExtensions<Product>.Count(_context.Products.Where(x => x.IsPrice == true), productParameters);

            if (count < productParameters.PageSize)
            {
                viewModel.Products = PagedList<Product>.ToPagedList(_context.Products.Where(x => x.IsPrice == true), productParameters.PageNumber, productParameters.PageSize);

                var product_2 = PagedList<Product>
                    .ToPagedList(_context.Products
                .Include(p => p.CommodityGroup)
                .Where(p =>  p.IsPrice == false), productParameters.PageNumber, productParameters.PageSize);

                foreach (var item in product_2)
                {
                    viewModel.Products.Add(item);
                }

                ViewData["PageNumber"] = "1";

                return View(viewModel);
            }

            viewModel.Products = PagedList<Product>.ToPagedList(_context.Products.Where(x => x.IsPrice == true), productParameters.PageNumber, productParameters.PageSize);

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

            var products = _context.Products
                .Include(p => p.CommodityGroup)
                .Where(p => listGroups.Contains(p.CommodityGroupId) && p.IsPrice == true);



            var productParameters = new ProductParameters();
            
            var count = PaginExtensions<Product>.Count(products, productParameters);

            if (count < productParameters.PageSize)
            {
                viewModel.Products = PagedList<Product>.ToPagedList(products, productParameters.PageNumber, productParameters.PageSize);

                var product_2 = PagedList<Product>
                    .ToPagedList(_context.Products
                .Include(p => p.CommodityGroup)
                .Where(p => listGroups.Contains(p.CommodityGroupId) && p.IsPrice == false), productParameters.PageNumber, productParameters.PageSize);

                foreach (var item in product_2)
                {
                    viewModel.Products.Add(item);
                }

                ViewData["PageNumber"] = "1";

                return View(viewModel);
            }

            viewModel.Products = PagedList<Product>.ToPagedList(products, productParameters.PageNumber, productParameters.PageSize);


            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> ShowCardProductPartial(int pageNumber, string groupId , int pageIsPireNumber)
        {
            
            var queryables = _context.Products.Include(p => p.CommodityGroup);
            
            var groups = await GetCommodidtyGroups();

           
            var curentgroup = FindGroup(groups.ToList(), groupId);

            var listGroups = new List<string>() { groupId };

            if (groupId != null)
            {
                SerialGroups(curentgroup, listGroups);
            }


            var reponeProduct = new ReponeProduct() {};
        

            var lists = GetPagedListProducts(queryables , listGroups, pageNumber, reponeProduct, groupId, pageIsPireNumber);

            PartialViewResult partialViewResult = PartialView("_CardProducts", lists);
            
            string viewContent = PartialViewToString.ConvertViewToString(this.ControllerContext, partialViewResult, _viewEngine);

            reponeProduct.ReturnHtml = viewContent;
            
            return Json(reponeProduct);
        }

        [NonAction]
        public PagedList<Product> GetPagedListProducts( IQueryable<Product> queryables, List<string> listGroups, int pageNumber, ReponeProduct reponeProduct, string groupId , int pageIsPireNumber)
        {
            var productParameters = new ProductParameters() { PageNumber = pageNumber };

           
            if (groupId != null)
            {
                var count = PaginExtensions<Product>.Count(queryables.Where(x => listGroups.Contains(x.CommodityGroupId) && x.IsPrice == true), productParameters);
                if (count > 0)
                { 
                    var listProduct_1 = PagedList<Product>
                       .ToPagedList(queryables.Where(x => listGroups.Contains(x.CommodityGroupId) && x.IsPrice == true)
                       , productParameters.PageNumber, productParameters.PageSize);

                    if (listProduct_1.Count() < productParameters.PageSize)
                    {
                        var itemProducts = PagedList<Product>
                        .ToPagedList(queryables.Where(x => listGroups.Contains(x.CommodityGroupId) && x.IsPrice == false)
                        , productParameters.PageNumber, productParameters.PageSize);

                        foreach (var item in itemProducts)
                        {
                            listProduct_1.Add(item);
                        }

                        reponeProduct.PageNumber = 1;
                        reponeProduct.IsPrice = true;

                        return listProduct_1;
                    }
                    reponeProduct.PageNumber = 0;
                    reponeProduct.IsPrice = true;
                    return listProduct_1;
                }


                var listProduct_2 = PagedList<Product>
                       .ToPagedList(queryables.Where(x => listGroups.Contains(x.CommodityGroupId) && x.IsPrice == false)
                       , pageIsPireNumber, productParameters.PageSize);

                return listProduct_2;

            }

            var count_1 = PaginExtensions<Product>.Count(queryables.Where(x => x.IsPrice == true), productParameters);

            if (count_1 > 0)
            {
                var listProduct_3 = PagedList<Product>
                   .ToPagedList(queryables.Where(x => x.IsPrice == true)
                   , productParameters.PageNumber, productParameters.PageSize);

                if (listProduct_3.Count() < productParameters.PageSize)
                {
                    var itemProducts = PagedList<Product>
                    .ToPagedList(queryables.Where(x => x.IsPrice == false)
                    , productParameters.PageNumber, productParameters.PageSize);

                    foreach (var item in itemProducts)
                    {
                        listProduct_3.Add(item);
                    }

                    reponeProduct.PageNumber = 1;
                    reponeProduct.IsPrice = true;

                    return listProduct_3;
                }
                reponeProduct.IsPrice = true;
                reponeProduct.PageNumber = 0;
                return listProduct_3;
            }

            var listProduct_4 = PagedList<Product>
                   .ToPagedList(queryables.Where(x => x.IsPrice == false)
                   , pageIsPireNumber, productParameters.PageSize);

            return listProduct_4;

        }



        [HttpGet]
        public async Task<IActionResult> Detail([FromRoute]string id)
        {

            var product = _context.Products
                .Include(p => p.CommodityGroup)
                .Include(p => p.Commodity
                ).Include(p => p.Images).Include(x => x.Wholesale).FirstOrDefault(p => p.Id == id);

            if (product is null)
            {
                return NotFound("Không tìm thấy dữ liệu ID");
            }


            var productDetail = ObjectMapper.Mapper.Map<ProductDetailDto>(product);
            
           
            var au = await _authorizationService.AuthorizeAsync(User, null,
                                                          new CanOptionWholesaleRequirements());

            if (au.Succeeded)
            {
                if (productDetail.Wholesale != null)
                {
                    productDetail.Price = product.Wholesale.Price;
                    productDetail.Promotion = product.Wholesale.Promotion;
                    productDetail.isWholesale = true;

                }
            }

            var groups = await GetCommodidtyGroups();

            var curentgroup = FindGroup(groups.ToList(), product.CommodityGroupId);

            var listGroups = new List<string>() { product.CommodityGroupId };

            if (product.CommodityGroupId != null)
            {
                SerialGroups(curentgroup, listGroups);
            }


            var products = _context.Products
                .Include(p => p.CommodityGroup)
                .Where(p => listGroups.Contains(p.CommodityGroupId) && p.IsPrice == true);

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
                ProductDetailDto = productDetail,
                ReturnUrl = Domain()
            };

            var productParameters = new ProductParameters();
     

            var count = PaginExtensions<Product>.Count(products, productParameters);

            if (count < productParameters.PageSize)
            {
                viewModel.Products = PagedList<Product>.ToPagedList(products, productParameters.PageNumber, productParameters.PageSize);

                var product_2 = PagedList<Product>
                    .ToPagedList(_context.Products
                .Include(p => p.CommodityGroup)
                .Where(p => listGroups.Contains(p.CommodityGroupId) && p.IsPrice == false), productParameters.PageNumber, productParameters.PageSize);

                foreach (var item in product_2)
                {
                    viewModel.Products.Add(item);
                }

                ViewData["PageNumber"] = "1";
                ViewData["curenturl"] = HttpContextAccessorPathDomainFull();
                return View(viewModel);
            }

            ViewData["PageNumber"] = "0";
            ViewData["curenturl"] = HttpContextAccessorPathDomainFull();
            viewModel.Products = PagedList<Product>.ToPagedList(products, productParameters.PageNumber, productParameters.PageSize);
          
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
        public async Task<IActionResult> Selling()
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
           
            viewModel.ProductHots = PagedList<Product>
                .ToPagedList(_context.Products.Include(p => p.CommodityGroup)
                .Include(p => p.Commodity).Include(x => x.Images)
                .Where(x => x.Hot == true), productParameters.PageNumber, productParameters.PageSize);
            var product = await _context.Products.Where(x => x.Hot == true).ToListAsync();



            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Promotion()
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

            viewModel.ProductHots = PagedList<Product>
                .ToPagedList(_context.Products.Include(p => p.CommodityGroup)
                .Include(p => p.Commodity).Include(x => x.Images)
                .Where(x => x.Hot == true), productParameters.PageNumber, productParameters.PageSize);
            var product = await _context.Products.Where(x => x.IsEvent == true).ToListAsync();



            return View(viewModel);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        
    }
}