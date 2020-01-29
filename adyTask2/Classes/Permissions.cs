using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using adyTask2.Models;


namespace adyTask2.Classes
{
    public class Permissions
    {
        private HttpContext Context { get; }

        public Permissions(HttpContext _context)
        {
            Context = _context;
        }

        public bool IsPermitted(int page_id)
        {
            if (Context.User.IsInRole("Admin"))
                return true;

            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var user_id = int.Parse(Context.User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var permissions = adyContext.Permission.FirstOrDefault(x => x.UserId == user_id);

                if (permissions == null)
                    return false;

                var page_permissions = permissions.Page;
                var pages = page_permissions.Split('-');
                return pages.Contains(page_id.ToString());
            }
        }


    }
}
