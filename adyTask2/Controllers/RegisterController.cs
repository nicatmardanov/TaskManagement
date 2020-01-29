using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using adyTask2.Models;

namespace adyTask2.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        //[HttpPost]
        //public async Task<JsonResult> Index(User _user)
        //{
        //    using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
        //    {
        //        _user.Active = 1;
        //        adyContext.User.Add(_user);
        //        await adyContext.SaveChangesAsync();
        //    }
        //    return Json(new { res = "1" });
        //}

        //public JsonResult CheckMail(string email)
        //{
        //    using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
        //    {
        //        if (adyContext.User.FirstOrDefault(x => x.Email == email) == null)
        //            return Json(new { res = "1" });
        //    }
        //    return Json(new { string.Empty });
        //}
    }
}