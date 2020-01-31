using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using adyTask2.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace adyTask2.Controllers
{
    [Microsoft.AspNetCore.Authorization.Authorize]
    public class MeetingOperationController : Controller
    {

        private string IpAdress { get; }
        private string AInformation { get; }

        public MeetingOperationController(IHttpContextAccessor accessor)
        {
            IpAdress = !string.IsNullOrEmpty(accessor.HttpContext.Connection.RemoteIpAddress.ToString()) ? accessor.HttpContext.Connection.RemoteIpAddress.ToString() : "";
            AInformation = accessor.HttpContext.Request.Headers["User-Agent"].ToString();
        }

        //get
        public IActionResult Add(int id)
        {
            ViewBag.Id = id;
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                var _user = adyContext.User.FirstOrDefault(x => x.EmailAddress == User.Identity.Name);
                ViewBag.User = _user.FirstName + " " + _user.LastName;

                return View(adyContext.MeetingLine.Where(x => x.Id == id && x.StatusId > 1 && x.StatusId != 8 && ((x.ResponsibleEmail == _user.EmailAddress && x.IsDirected == 0) || (x.Direct.FirstOrDefault(y => y.ToUserId == _user.Id) != null && x.IsDirected == 1))).Include(x => x.Meeting).Include(x => x.MlTypeNavigation).Include(x => x.Status).ToList());
                //return View(adyContext.MeetingLine.Where(x => x.Id == id && x.ResponsibleEmail == User.Identity.Name || x.Direct.FirstOrDefault(y => y.ToUserId == _user.Id && y.IsActive == 1) != null).ToList());
            }
        }

        //post
        [HttpPost]
        public async Task Add(string[] moDescp, string[] moComp, bool[] moFilePos, int mlId)
        {
            using (adyTaskManagementContext adyContext = new adyTaskManagementContext())
            {
                //var _user = adyContext.User.FirstOrDefault(z => z.Email == User.Identity.Name);
                //if (adyContext.MeetingLine.FirstOrDefault(x => (x.ResponsibleEmail == User.Identity.Name && x.IsDirected == 0) || (x.Direct.FirstOrDefault(y => y.ToUserId == _user.Id) != null && x.IsDirected == 1)) != null)
                //{
                //}


                MlDetails _meetingOperation;

                //if (adyContext.MeetingLine.FirstOrDefault(x => x.Id == mlId).StatusId == 2)
                //{
                //    adyContext.MeetingLine.FirstOrDefault(x => x.Id == mlId).StatusId = 3;
                //    await adyContext.SaveChangesAsync();
                //}


                var moFiles = Request.Form.Files;
                int user_id = int.Parse(User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value);
                int j = 0;

                for (int i = 0; i < moDescp.Length; i++)
                {
                    _meetingOperation = new MlDetails
                    {
                        Description = moDescp[i],
                        Completion = moComp[i],
                        UserId = user_id,
                        CreateDate = DateTime.UtcNow.AddHours(4),
                        MlId = mlId
                    };

                    adyContext.MlDetails.Add(_meetingOperation);
                    await adyContext.SaveChangesAsync();

                    if (moFilePos[i])
                    {
                        await Classes.FileSave.Save(moFiles[j], _meetingOperation.Id, 3, adyContext);
                        j++;
                    }

                    Classes.Log _log = new Classes.Log();

                    await _log.LogAdd(3, moDescp[i], _meetingOperation.Id, 18, user_id, IpAdress, AInformation);


                }
            }
        }

        //partial
        public PartialViewResult MeetingOperationForm()
        {
            return PartialView();
        }
    }
}