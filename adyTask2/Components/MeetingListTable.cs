using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;


namespace adyTask2.Components
{
    public class MeetingListTable:ViewComponent
    {
        public IViewComponentResult Invoke(List<Meeting> meetings, int page)
        {
            ViewBag.Page = page;
            return View(meetings);
        }
    }
}
