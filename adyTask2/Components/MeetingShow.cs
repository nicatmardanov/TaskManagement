using adyTask2.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace adyTask2.Components
{
    public class MeetingShow:ViewComponent
    {
        public IViewComponentResult Invoke(Meeting meeting)
        {
            return View(meeting);
        }
    }
}
