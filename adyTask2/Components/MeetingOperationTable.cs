using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;
using Microsoft.EntityFrameworkCore;

namespace adyTask2.Components
{
    public class MeetingOperationTable : ViewComponent
    {
        public IViewComponentResult Invoke(int mlId, byte type)
        {
            ViewBag.mlId = mlId;
            ViewBag.Type = type;

            adyTaskManagementContext adyContext = new adyTaskManagementContext();

            IQueryable<MLog> m_logs = adyContext.MLog.Where(x => x.Type == 2 && x.RefId == mlId);

            var m_operations_id = adyContext.MlDetails.Where(x => x.MlId == mlId).Select(x => x.Id);

            m_logs = m_logs.Concat(adyContext.MLog.Where(x => x.Type == 3 && m_operations_id.Contains(x.RefId)));

            return View(m_logs.OrderByDescending(x => x.Id).Include(x => x.User).Include(x => x.Operation));
        }
    }
}
