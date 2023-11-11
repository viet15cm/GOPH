using GOPH.Entites;
using Microsoft.AspNetCore.Mvc;

namespace GOPH.Views.Shared.Components.MenuGroups
{
    public class MenuGroups  : ViewComponent
    {
        public class Menu
        {

            public string GroupId { get; set; }

            public List<string> listSerialUrl { get; set; }

            public IEnumerable<CommodityGroup> Groups { get; set; }

            public CommodityGroup CurentGroup { get; set; }


        }

        public const string COMPONENTNAME = "MenuGroups";
        public MenuGroups() { }

        public IViewComponentResult Invoke(Menu data)
        {
            return View(data);
        }
    }
}
