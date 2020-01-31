using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;

namespace adyTask2.Components
{
    public class MLTableAlt:ViewComponent
    {
        public IViewComponentResult Invoke(IQueryable<Classes.Meeting_Line> MLine)
        {
            return View(MLine);
        }
    }
}
