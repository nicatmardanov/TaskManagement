using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

using adyTask2.Models;

namespace adyTask2.Components
{
    public class UserName : ViewComponent
    {
        public string Invoke(string FullName)
        {
            return FullName;
        }

    }
}
