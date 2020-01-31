using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using adyTask2.Models;
using Microsoft.EntityFrameworkCore;

namespace adyTask2.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class DepartmentController : Controller
    {
        //Partial
        public PartialViewResult DepList(string id)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var _department = adyContext.Department.AsNoTracking().Where(x => x.Active == 1 && x.Name.Contains(id));
                return PartialView(_department.ToList());
            }
        }

        //JSON
        public JsonResult GetDepartments(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                adyTaskManagementContext adyContext = new adyTaskManagementContext();

                IQueryable<Department> _departments = adyContext.Department.Where(x => x.Name.ToLower().Contains(term.ToLower()) && x.Active == 1);

                var place_info = _departments.Select(x => new { id = x.Id, full_name = $"{x.Name}" });

                return Json(place_info);
            }
            return Json(string.Empty);
        }

        public async Task<JsonResult> GetUsersDepartments(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                adyTaskManagementContext adyContext = new adyTaskManagementContext();
                var user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                var permissions = await adyContext.Permission.FirstOrDefaultAsync(x => x.UserId == user_id);
                bool isNullDep = true;


                if (permissions != null)
                {
                    isNullDep = string.IsNullOrEmpty(permissions.Department);

                    if (!isNullDep)
                    {
                        IQueryable<Department> _departments = adyContext.Department.Where(x => x.Name.ToLower().Contains(term.ToLower()) && x.Active == 1);
                        var deps = Array.ConvertAll(permissions.Department.Split(';'), x => int.Parse(x));
                        _departments = _departments.Where(x => deps.Contains(x.Id));
                        var place_info = _departments.Select(x => new { id = x.Id, full_name = $"{x.Name}" });
                        return Json(place_info);
                    }
                    return Json(String.Empty);


                }
                else if (User.IsInRole("Admin"))
                {
                    IQueryable<Department> _departments = adyContext.Department.Where(x => x.Name.ToLower().Contains(term.ToLower()) && x.Active == 1);
                    var place_info = _departments.Select(x => new { id = x.Id, full_name = $"{x.Name}" });
                    return Json(place_info);
                }
            }
            return Json(String.Empty);
        }

    }
}