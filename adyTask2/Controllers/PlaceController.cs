using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using adyTask2.Models;
using Microsoft.AspNetCore.Mvc;

namespace adyTask2.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class PlaceController : Controller
    {
        //Json
        public JsonResult GetPlaces(string term)
        {
                adyTaskManagementContext adyContext = new adyTaskManagementContext();

                IQueryable<Place> _places = adyContext.Place.Where(x => x.Name.ToLower().Contains(term.ToLower()));

                var place_info = _places.Select(x => new { id= x.Id, full_name = $"{x.Name}"});

                return Json(place_info);
        }
    }
}