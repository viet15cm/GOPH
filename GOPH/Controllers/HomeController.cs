using GOPH.DbContextLayer;
using GOPH.Entites;
using GOPH.Models;
using GOPH.Paging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace GOPH.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController(IMemoryCache cache, AppDbContext appDbContext, ILogger<BaseController> logger, IHttpContextAccessor httpContextAccessor) : base(cache, appDbContext, logger, httpContextAccessor)
        {
        }

        public class ViewModel
        {
            public string GroupId { get; set; }

            public List<string> listSerialUrl { get; set; }

            public IEnumerable<CommodityGroup> Groups { get; set; }

            public CommodityGroup CurentGroup { get; set; }

            public IEnumerable<Product> Products { get; set; }

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
                Products = await _context.Products.Include(p => p.CommodityGroup).Include(p => p.Commodity).Take(15).ToListAsync(),
                ReturnUrl = Domain()
            };


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
                Products = products.AsQueryable().Take(15).ToList(),
               
                ReturnUrl = Domain()
            };

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
        public async Task<IActionResult> Detail(string id)
        {
            var product = _context.Products.Include(p => p.CommodityGroup).Include(p => p.Commodity).FirstOrDefault(p => p.Id == id);
            if (product is null)
            {
                return NotFound("Không tìm thấy dữ liệu ID");
            }

            var groups = await GetCommodidtyGroups();

            var curentgroup = FindGroup(groups.ToList(), product.CommodityGroup.Id);

            var listGroups = new List<string>() { };

            if (product.CommodityGroup.Id != null)
            {
                SerialGroups(curentgroup, listGroups);
            }

            var products = _context.Products.Include(p => p.CommodityGroup).Where(p => listGroups.Contains(p.CommodityGroupId));

            if (curentgroup is null)
            {
                return NotFound($"Không tìm thấy {product.CommodityGroup.Id}");
            }


            var listSerialUrl = new List<string>();

            FindPostBySlug(groups.ToList(), product.CommodityGroup.Id, listSerialUrl); // chỉnh lại code

            var viewModel = new ViewModel()
            {
                GroupId = product.CommodityGroup.Id,
                listSerialUrl = listSerialUrl,
                Groups = groups.ToList(),
                CurentGroup = curentgroup,
                CurentProduct = product,
                Products = products.AsQueryable().Take(15).ToList(),
                ReturnUrl = Domain()
            };

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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}