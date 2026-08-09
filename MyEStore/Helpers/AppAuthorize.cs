using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyEStore.Helpers
{
    public class AppAuthorize : ActionFilterAttribute
    {
        private readonly string _role;
        public AppAuthorize(string role)
        {
            _role = role;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var userRole = session.GetString("VaiTro");
            var maKh = session.GetString("MaKH");

            
            if (string.IsNullOrEmpty(maKh))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            
            if (userRole != _role)
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}