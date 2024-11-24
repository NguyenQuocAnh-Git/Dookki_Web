using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dookki_Web.App_Start
{
    public class RoleUser : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var user = SessionConfig.GetUser();
            if(user == null)
            {
                //Dieu huong ve trang login
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new
                    {
                        controller = "AdminHome",
                        action = "Login",
                        area = "Admin"
                    }));
                return;
            }
            return;
        }
    }
}