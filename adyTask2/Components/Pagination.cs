using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adyTask2.Components
{
    public class Pagination:ViewComponent
    {
        public IViewComponentResult Invoke(int maxPage, int pageNumber)
        {
            ViewBag.MaxPage = maxPage;
            ViewBag.PageNumber = pageNumber;
            //ViewBag.MeetingLine = meetingLine;
            //ViewBag.NotCompleted = notCompleted;
            //ViewBag.MyMeetingLines = myMeetingLines;
            //ViewBag.Department = department;
            //ViewBag.Type = type;

            return View();
        }
    }
}
