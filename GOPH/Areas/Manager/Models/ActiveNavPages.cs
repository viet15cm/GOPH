using Microsoft.AspNetCore.Mvc.Rendering;

namespace GOPH.Areas.Manager.Models
{
    public static class ActiveNavPages
    {
        //ConfirmedEmail
        //ChangeEmail
        public static string Order => "Order";
        
        public static string Invoice => "Invoice";

        public static string Product => "Product";

        public static string Commodity => "Commodity";

        public static string Group => "Group";

        public static string Cache => "Cache";


        public static string Event => "Event";


        public static string OrderRecycleBin => "OrderRecycleBin";

        public static string OrderActiveClass(ViewContext viewContext) => PageNavClass(viewContext, Order);
        public static string InvoiceActiveClass(ViewContext viewContext) => PageNavClass(viewContext, Invoice);
        public static string ProductActiveClass(ViewContext viewContext) => PageNavClass(viewContext, Product);
        public static string CommodityActiveClass(ViewContext viewContext) => PageNavClass(viewContext, Commodity);
        public static string GroupActiveClass(ViewContext viewContext) => PageNavClass(viewContext, Group);
        public static string CacheActiveClass(ViewContext viewContext) => PageNavClass(viewContext, Cache);

        public static string EventActiveClass(ViewContext viewContext) => PageNavClass(viewContext, Event);

        public static string OrderRecycleBinActiveClass(ViewContext viewContext) => PageNavClass(viewContext, OrderRecycleBin);

        private static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActiveNavbar"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);
            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
