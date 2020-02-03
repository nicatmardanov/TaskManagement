using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using adyTask2.Models;

namespace adyTask2.Classes.Attributes
{
    //[AttributeUsage(AttributeTargets.Method, AllowMultiple =false,Inherited =false)]
    public class PermissionAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private int PageId { get; }
        public PermissionAttribute(int _PageId)
        {
            PageId = _PageId;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated)
                context.Result = new RedirectToRouteResult(
                new RouteValueDictionary {{ "Controller", "Login" },
                                              { "Action", "Index" } });
            else
            {
                using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
                {
                    var user_id = int.Parse(context.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                    var permissions = adyContext.Permission.FirstOrDefault(x => x.UserId == user_id);
                    if (permissions != null)
                    {
                        string[] pages = permissions.Page.Split(';');
                        if (!(pages.Contains(PageId.ToString()) || context.HttpContext.User.IsInRole("Admin")))
                        {
                            context.Result = new RedirectToRouteResult(
                            new RouteValueDictionary {{ "Controller", "Home" },
                                              { "Action", "Index" } });
                        }
                    }
                    else if (!context.HttpContext.User.IsInRole("Admin"))
                    {
                        context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary {{ "Controller", "Home" },
                                              { "Action", "Index" } });
                    }


                }
            }

        }

        //////////////////////////////

        //context.Result = new RedirectToRouteResult(
        //    new RouteValueDictionary {{ "Controller", "Home" },
        //                              { "Action", "Index" } });




    }
}