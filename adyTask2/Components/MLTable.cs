using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;
using Microsoft.EntityFrameworkCore;

namespace adyTask2.Components
{
    public class MLTable : ViewComponent
    {
        public IViewComponentResult Invoke(List<MeetingLine> _meetingLine, byte type, int page)
        {
            ViewBag.Type = type;
            ViewBag.Page = page;

            return View(_meetingLine);
        }
    }
}
