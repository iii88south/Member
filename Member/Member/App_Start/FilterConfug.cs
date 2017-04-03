using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Member.App_Start
{
    public class FilterConfig 
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeAttribute());
        }

        

    }

    public class AuthorizationFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            string userId = HttpContext.Current.User.Identity.GetUserId();

            if (userId ==null)
            {
                var controller = HttpContext.Current.Request.RequestContext.RouteData.Values["Controller"];
                if (controller !="Auth")
                {
                    filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary() {
                    { "Controller","Auth"},
                    { "Action","Login"}

                });
                }
           

            }

            base.OnAuthorization(filterContext);
        }
    }
}