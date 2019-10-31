using System.Web;
using System.Web.Mvc;

namespace ProjectManagement_cum_feedback_systemMVC
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
