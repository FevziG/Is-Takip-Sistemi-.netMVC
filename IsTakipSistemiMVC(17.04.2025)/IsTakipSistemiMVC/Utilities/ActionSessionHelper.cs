using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace IsTakipSistemiMVC.Utilities
{
    public class ActionSessionHelper : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            int? userId = httpContext.Session.GetInt32("personelId");
            int? yetkiId = httpContext.Session.GetInt32("personelYetkiTurId");
            
            if (userId == null || yetkiId != 1)
            {
                context.Result = new RedirectToActionResult("LoginPage", "Home", null);
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Şu an için işlem yapılmıyor, ancak implement etmek zorundayız.
        }
    }
}
