using System.Web.Mvc;

namespace AnvilGroup.Applications.Fit2Work {
    public class FilterConfig {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
