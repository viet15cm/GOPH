
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

using System.Data;
using GOPH.Entites;
using GOPH.DbContextLayer;
using GOPH.Paging;
using IronXL;
using GOPH.Migrations;

namespace GOPH.Areas.Identity.Pages.User
{

    public class IndexModel : UserPageModel
    {
      
        [BindProperty]
        public string SearchUserName { get; set; }
        
      
        
        public IndexModel(UserManager<AppUser> userManager, ILogger<UserPageModel> logger, AppDbContext dbContext , RoleManager<IdentityRole> roleManager) : base(userManager, logger, dbContext, roleManager)
        {

        }

        public PagedList<UserAndRole> users { get; set; }

       public class UserAndRole : AppUser
        {
            public string UserRoles { get; set; }
        }

        public async Task<IActionResult> OnGet([FromQuery] UserParameters userParameters)
        {
           
            PagedList<AppUser> pageUsers = PagedList<AppUser>.ToPagedList(_userManager.Users,
                        userParameters.PageNumber,
                        userParameters.PageSize);

            List<UserAndRole> derivedList = pageUsers.ToList().Select(x => new UserAndRole
            {

                Id = x.Id,
                UserName = x.UserName,
                PhoneNumber = x.PhoneNumber,
                LastName = x.LastName,
                FirstName = x.FirstName,
                CurrentPoint = x.CurrentPoint,
                

            }).ToList();

            users = new PagedList<UserAndRole>(derivedList, pageUsers.TotalCount, userParameters.PageNumber, userParameters.PageSize);

            foreach (var item in users)
            {
                var roles = await _userManager.GetRolesAsync(item);
                item.UserRoles = string.Join(",", roles);
                
            }

            return Page();

        }

        public async Task<IActionResult> OnPostImportCustomer()
        {
            var users = new List<AppUser>();
            WorkBook wb = WorkBook.Load("D://customerimport.xlsx");
            DataSet ds = wb.ToDataSet();//behave complete Excel file as DataSet
            foreach (DataTable dt in ds.Tables)//behave Excel WorkSheet as DataTable. 
            {
                int k = 1;

                foreach (DataRow row in dt.Rows)//corresponding Sheet's Rows
                {
                    var user = new AppUser();

                    if (k != 1)
                    {

                        for (int h = 0; h < dt.Columns.Count; h++)//Sheet columns of corresponding row
                        {
                            if (h == 0)
                            {
                                user.FirstName = row[h].ToString();
                            }

                            if (h == 1)
                            {
                                user.UserName = row[h].ToString();
                                user.PhoneNumber = row[h].ToString();
                            }

                            if (h == 2)
                            {
                                user.CurrentPoint = int.Parse(row[h].ToString());
                            }

                        }

                        users.Add(user);
                    }
                    k++;

                }
            }

            try
            {
                foreach (var user in users)
                {

                   var result =  await _userManager.CreateAsync(user, user.PhoneNumber);

                   if (result.Succeeded)
                    {
                        
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            StatusMessage = $"import thành công danh sách các tài khoản";

            return RedirectToPage("index");

        }

    }
}
