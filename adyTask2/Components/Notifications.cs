using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace adyTask2.Components
{
    public class Notifications : ViewComponent
    {
        public IViewComponentResult Invoke(IQueryable<MLog> logs, int page, int user_id)
        {
            ViewBag.PageNumber = page;
            //ViewBag.MaxPage = (int)page_count;
            ViewBag.UserId = user_id;

            return View(logs.OrderByDescending(x => x.Id).Include(x => x.Operation).Include(x => x.User).Skip((page - 1) * 10).Take(10));
        }
    }
}
