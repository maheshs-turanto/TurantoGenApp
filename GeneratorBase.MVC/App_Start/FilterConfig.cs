using System.Web;
using System.Web.Mvc;
namespace GeneratorBase.MVC
{
/// <summary>A filter configuration.</summary>
public class FilterConfig
{
    /// <summary>Registers the global filters described by filters.</summary>
    ///
    /// <param name="filters">The filters.</param>
    
    public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    {
        filters.Add(new HandleErrorAttribute());
        filters.Add(new HandleAntiforgeryTokenErrorAttribute()
        {
            ExceptionType = typeof(HttpAntiForgeryException)
        });
    }
}
}
