using adyTask2.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adyTask2.Components
{
    public class MeetingOperationLog : ViewComponent
    {
        public IViewComponentResult Invoke(int refId)
        {
            ViewBag.refId = refId;
            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            return View(adyContext.MLog.Where(x=>x.RefId==refId && x.Type==2));
        }
    }
}