using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.DirectoryServices;
using adyTask2.Models;

namespace adyTask2.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        private bool ADLogin(string email, string password)
        {
            string path = "LDAP://local.ady.az";

            string description = "";

            using (DirectoryEntry entry = new DirectoryEntry(path, email.Split("@")[0], password))

            {

                DirectorySearcher searcher = new DirectorySearcher(entry)

                {

                    Filter = "(objectclass=user)"

                };

                try

                {

                    SearchResult ent = searcher.FindOne();

                    return true;

                }

                catch (Exception exc)

                {

                    description = exc.Message;

                    return false;

                }

            }
        }

        [HttpPost]
        public async Task<JsonResult> Login(string username, string password)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                //var _user = adyContext.User.FirstOrDefault(x => x.EmailAddress == email && x.Password == password);

                var email = username.Contains("@ady.az") ? username : username + "@ady.az";

                Models.User _user = adyContext.User.FirstOrDefault(x => x.EmailAddress == email);
                bool isAD = _user.IsActiveDirectory.HasValue && _user.IsActiveDirectory.Value;

                if (ADLogin(email, password) || (_user.Password == password && !isAD))
                {
                    ClaimsIdentity claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, email));
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, _user.PersonId.ToString()));
                    claimsIdentity.AddClaim(new Claim("FullName", _user.FirstName + " " + _user.LastName));


                    foreach (var item in _user.UserRole)
                    {
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, item.Role.RoleName));
                    }

                    AuthenticationProperties authProperty = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddYears(1),
                        IssuedUtc = DateTime.UtcNow
                    };

                    var principial = new ClaimsPrincipal(claimsIdentity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principial, authProperty);
                    return Json(new { res = "1" });
                }
            }
            return Json(new { res = "0" });
        }


        [Microsoft.AspNetCore.Authorization.Authorize]
        public async Task<IActionResult> SignOut()
        {
            await HttpContext.SignOutAsync();

            return RedirectToAction("Index", "Login");
        }
    }
}