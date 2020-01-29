using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;
using Microsoft.EntityFrameworkCore;

namespace adyTask2.Components
{
    public class Notifications : ViewComponent
    {
        public IViewComponentResult Invoke(int page, int user_id)
        {
            adyTaskManagementContext adyContext = new adyTaskManagementContext();
            var meeting_lines = adyContext.MeetingLine.Where(x => ((x.ResponsibleEmail == User.Identity.Name && x.StatusId > 1 && x.IsPublished == 1) || (x.CreatorId == user_id && x.StatusId >= 1)) || x.Direct.FirstOrDefault(y => y.ToUserId == user_id && y.IsActive == 1) != null || (x.FollowerEmail == User.Identity.Name && x.StatusId > 1) || (x.IdentifierEmail == User.Identity.Name && x.StatusId > 1)).Select(x => x.Id);
            var logs = adyContext.MLog.Where(x => x.ReadDate == null && x.Type == 2 && x.OperationId!=6 && meeting_lines.Contains(x.RefId)).OrderByDescending(x => x.Id).Skip((page - 1) * 10).Take(10); ;

            ViewBag.PageNumber = page;


            return View(logs.OrderByDescending(x => x.Id).Include(x => x.Operation).Include(x => x.User));
        }
    }
}
