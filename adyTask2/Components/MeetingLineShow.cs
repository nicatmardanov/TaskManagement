using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;

namespace adyTask2.Components
{
    public class MeetingLineShow:ViewComponent
    {
        public IViewComponentResult Invoke(MeetingLine _meetingLine)
        {
            return View(_meetingLine);
        }
    }
}
