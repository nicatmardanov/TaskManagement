using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;
using Microsoft.AspNetCore.Mvc;

namespace adyTask2.Controllers
{
    public class PositionController : Controller
    {
        public JsonResult Positions(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                adyTaskManagementContext adyContext = new adyTaskManagementContext();

                var positions = adyContext.PositionOthers.Where(x => x.Name.ToLower().Contains(term.ToLower()));
                var positions_info = positions.Select(x => new { id = x.Id, name = x.Name });

                return Json(positions_info);
            }


            return Json(string.Empty);
        }
    }
}