using System.Web;
using System.Web.Mvc;
using TradingAPI.Controllers;

namespace TradingAPI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            
            filters.Add(new HandleErrorAttribute());
            
        }
    }
}