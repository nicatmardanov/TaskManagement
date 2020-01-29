using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;
using Microsoft.AspNetCore.Mvc;

namespace adyTask2.Controllers
{
    public class CountryController : Controller
    {
        public JsonResult Countries(string term)
        {
                adyTaskManagementContext adyContext = new adyTaskManagementContext();

                IQueryable<Country> countries_result = adyContext.Country.Where(x => x.Name.ToLower().Contains(term.ToLower()));
                var country_info = countries_result.Select(x => new { id = x.Id, name = x.Name});

                return Json(country_info);
        }
    }
}