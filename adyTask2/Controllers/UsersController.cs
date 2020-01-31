using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using adyTask2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace adyTask2.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {

        private string IpAdress { get; }
        private string AInformation { get; }

        public UsersController(IHttpContextAccessor accessor)
        {
            IpAdress = !string.IsNullOrEmpty(accessor.HttpContext.Connection.RemoteIpAddress.ToString()) ? accessor.HttpContext.Connection.RemoteIpAddress.ToString() : "";
            AInformation = accessor.HttpContext.Request.Headers["User-Agent"].ToString();
        }

        //get
        [Authorize(Roles = "Admin")]
        public IActionResult Permissions()
        {
            return View();
        }



        //post
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task Permissions(Permission _permission)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var permission = await adyContext.Permission.FirstOrDefaultAsync(x => x.UserId == _permission.UserId);
                if (permission == null)
                {
                    adyContext.Permission.Add(_permission);
                }
                else if (await TryUpdateModelAsync(permission))
                {
                    permission = _permission;
                }
                await adyContext.SaveChangesAsync();

                int user_id = _permission.UserId ?? default;

                Classes.Log _log = new Classes.Log();
                await _log.LogAdd(4, "", _permission.Id, 16, user_id, IpAdress, AInformation);
            }
        }

        [HttpPost]
        public async Task<string> AddOthers(OtherParticipiants Other_Participiants, string CountryName, string PositionName)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                if (!string.IsNullOrEmpty(CountryName))
                {
                    Country NewCountry = new Country { Name = CountryName };
                    adyContext.Country.Add(NewCountry);
                    await adyContext.SaveChangesAsync();

                    Other_Participiants.CountryId = NewCountry.Id;
                }

                if (!string.IsNullOrEmpty(PositionName))
                {
                    PositionOthers Position = new PositionOthers { Name = PositionName };
                    adyContext.PositionOthers.Add(Position);
                    await adyContext.SaveChangesAsync();

                    Other_Participiants.PositionId = Position.Id;
                }

                adyContext.OtherParticipiants.Add(Other_Participiants);
                await adyContext.SaveChangesAsync();

                return Other_Participiants.Email;
            }
        }


        //Json
        public JsonResult GetUsers(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                string[] searched_items = term.Split(' ');
                if (searched_items.Length > 0 && searched_items.Length < 3)
                {
                    adyTaskManagementContext adyContext = new adyTaskManagementContext();

                    IQueryable<User> _users = adyContext.User.Where(x => x.Active == 1 && (x.FirstName.ToLower().StartsWith(searched_items[0].ToLower()) || x.LastName.ToLower().StartsWith(searched_items[0].ToLower())));

                    if (searched_items.Length == 2)
                        _users = _users.Where(x => x.FirstName.ToLower().StartsWith(searched_items[1].ToLower()) || x.LastName.ToLower().StartsWith(searched_items[1].ToLower()));

                    var user_info = _users.Select(x => new { id = x.PersonId, email = x.EmailAddress, full_name = $"{x.FirstName} {x.LastName} ({x.OrganizationFullName})" });

                    return Json(user_info);
                }
            }
            return Json(String.Empty);
        }

        public JsonResult GetNonAdmins(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                string[] searched_items = term.Split(' ');
                if (searched_items.Length > 0 && searched_items.Length < 3)
                {
                    adyTaskManagementContext adyContext = new adyTaskManagementContext();

                    IQueryable<User> _users = adyContext.User.Where(x => x.Active == 1 && x.UserRole.FirstOrDefault(x => x.RoleId == 2) == null && (x.FirstName.ToLower().StartsWith(searched_items[0].ToLower()) || x.LastName.ToLower().StartsWith(searched_items[0].ToLower())));

                    if (searched_items.Length == 2)
                        _users = _users.Where(x => x.FirstName.ToLower().StartsWith(searched_items[1].ToLower()) || x.LastName.ToLower().StartsWith(searched_items[1].ToLower()));

                    var user_info = _users.Select(x => new { id = x.PersonId, email = x.EmailAddress, full_name = $"{x.FirstName} {x.LastName} ({x.OrganizationFullName})" });

                    return Json(user_info);
                }
            }

            return Json(String.Empty);
        }

        public JsonResult GetOtherParticipants(string term)
        {
            if (!string.IsNullOrEmpty(term))
            {
                string[] searched_items = term.Split(' ');
                if (searched_items.Length > 0 && searched_items.Length < 3)
                {
                    adyTaskManagementContext adyContext = new adyTaskManagementContext();

                    IQueryable<OtherParticipiants> _users = adyContext.OtherParticipiants.Where(x => x.Name.ToLower().StartsWith(searched_items[0].ToLower()) || x.Surname.ToLower().StartsWith(searched_items[0].ToLower()));

                    if (searched_items.Length == 2)
                        _users = _users.Where(x => x.Name.ToLower().StartsWith(searched_items[1].ToLower()) || x.Surname.ToLower().StartsWith(searched_items[1].ToLower()));

                    var user_info = _users.Select(x => new { id = x.Id, full_name = $"{x.Name} {x.Surname} ({x.Company})" });

                    return Json(user_info);
                }
            }
            return Json(String.Empty);
        }



        //partial
        public PartialViewResult PermissionListBox(int id)
        {

            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                if (adyContext.Permission.Count() > 0)
                {
                    return PartialView(adyContext.Permission.FirstOrDefault(x => x.UserId == id));
                }

                return PartialView(new Permission());
            }
        }


    }
}