using System.Web;
using System.Web.Mvc;

namespace AnvilGroup.Services.Fit2WorkApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
